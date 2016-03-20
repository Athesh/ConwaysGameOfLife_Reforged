using System;
using System.Linq;
using Praus.ConwaysGameOfLife.Utils;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class QNode {                
        /// <summary>
        /// Levá horní buňka
        /// </summary>
        public QNode TopLeft { get; }
        /// <summary>
        /// Pravá horní buňka
        /// </summary>
        public QNode TopRight { get; }
        /// <summary>
        /// Levá dolní buňka
        /// </summary>
        public QNode BotLeft { get; }
        /// <summary>
        /// Pravá dolní buňka
        /// </summary>
        public QNode BotRight { get; }

        /// <summary>
        /// úroveň buňky
        /// </summary>
        public int Level { get; }
        /// <summary>
        /// Zacachovaná populace v buňce
        /// </summary>
        public int Population { get; }
        /// <summary>
        /// Určuje jestli je list (nakonci stromu)
        /// </summary>
        public bool IsLeaf { get; }
        /// <summary>
        /// Jestli je život v buňce
        /// </summary>
        public bool IsAlive { get; }

        /// <summary>
        /// cache do které se cachujou buňky (když už ho mám, nevytvářím znova)
        /// </summary>
        private static Dictionary<QNode, QNode> cache { get; } = new Dictionary<QNode, QNode>();
        /// <summary>
        /// cache pro další generaci
        /// </summary>
        private QNode CacheResult { get; set; }

        /// <summary>
        /// privátní konstruktor buňky (Návrhový vzor factory)
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="botLeft"></param>
        /// <param name="botRight"></param>
        protected QNode(QNode topLeft, QNode topRight, QNode botLeft, QNode botRight) {
            QNode[] nodes = { topLeft, topRight, botLeft, botRight };
            if (nodes.Any(node => node == null)) { //check proti vytvoření nevalidního nodu
                throw new ArgumentNullException($"Any of these {nameof(TopLeft)}, {nameof(TopRight)}, " +
                    $"{nameof(BotLeft)}, {nameof(BotRight)}.", "Any of the given nodes cannot be null!");
            } else if (nodes.Any(node => node.Level != topLeft.Level)) {
                throw new ArgumentException("The given nodes do not have the same RootDistance!");
            }

            TopLeft = topLeft;
            TopRight = topRight;
            BotLeft = botLeft;
            BotRight = botRight;

            Level = topLeft.Level + 1;
            Population = nodes.Select(node => node.Population).Sum();

            IsLeaf = false;
            IsAlive = Population > 0;
        }

        /// <summary>
        /// Konstruktor, vytváří mrtvou/živou buňku
        /// </summary>
        /// <param name="alive"></param>
        protected QNode(bool alive) {
            TopLeft = TopRight = BotLeft = BotRight = null;
            Level = 0;
            Population = alive ? 1 : 0;         //nastavování vlastností
            IsAlive = alive;
            IsLeaf = true;
        }

        /// <summary>
        /// Factory metoda pro vytváření instance stromu
        /// </summary>
        /// <returns></returns>
        public static QNode Create() {
            return new QNode(false).CreateEmptyTree(3);
        }

        /// <summary>
        /// Factory metoda pro vytváření instance jednotlivé buňky
        /// </summary>
        /// <param name="TopLeft"></param>
        /// <param name="TopRight"></param>
        /// <param name="BotLeft"></param>
        /// <param name="BotRight"></param>
        /// <returns></returns>
        public QNode Create(QNode TopLeft, QNode TopRight, QNode BotLeft, QNode BotRight) {
            return new QNode(TopLeft, TopRight, BotLeft, BotRight).Intern(); //veme 4 buňky a spojí je do jedné
        }

        /// <summary>
        /// Factory metoda pro vytváření instance jednotlivé buňky (overload)
        /// </summary>
        /// <param name="alive"></param>
        /// <returns></returns>
        public QNode Create(bool alive) {
            return new QNode(alive).Intern();
        }

        /// <summary>
        /// Factory metoda pro vytváření instance prázdného stromu do určitého levelu
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public QNode CreateEmptyTree(int level) {
            if (level == 0) {
                return Create(false);
            }
            var emptyNode = CreateEmptyTree(level - 1);
            return Create(emptyNode, emptyNode, emptyNode, emptyNode);
        }

        /// <summary>
        /// Rozšiřuje plochu
        /// </summary>
        /// <returns></returns>
        public QNode ExpandTree() {
            var emptySpace = CreateEmptyTree(Level - 1); //vytvoření prázdného místa a napojení na existující tree
            return Create(
                Create(emptySpace, emptySpace, emptySpace, TopLeft),
                Create(emptySpace, emptySpace, TopRight, emptySpace),
                Create(emptySpace, BotLeft, emptySpace, emptySpace),
                Create(BotRight, emptySpace, emptySpace, emptySpace)
            );
        }

        /// <summary>
        /// Nastaví živou buňku na souřadnicích X, Y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public QNode SetCell(int x, int y) {
            if (Level == 0) {   //automaticky vytváří novou živou buňku
                return Create(true);
            }
            int d = Convert.ToInt32(Math.Pow(2, Level - 2));
            if (x < 0) {
                if (y < 0) {
                    return Create(TopLeft.SetCell(x + d, y + d), TopRight, BotLeft, BotRight);
                } else {
                    return Create(TopLeft, TopRight, BotLeft.SetCell(x + d, y - d), BotRight);
                }
            } else {
                if (y < 0) {
                    return Create(TopLeft, TopRight.SetCell(x - d, y + d), BotLeft, BotRight);
                } else {
                    return Create(TopLeft, TopRight, BotLeft, BotRight.SetCell(x - d, y - d));
                }
            }
        }

        /// <summary>
        /// Vrátí buňku ze souřadnic X, Y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public QNode GetCell(int x, int y) {
            if (Level == 0) {   //nemůžeme se dotázat na buňku které je mimo vesmír
                return this;
            }
            int d = Convert.ToInt32(Math.Pow(2, Level - 2));    //Velikost buňky je vždy mocnina 2,
                                                                //o kolik musíme posunout souřadnice aby jsme se dotali na souřadnice nové buňky
            if (x < 0) {    //určuju ve ktere čtvrtině je souřadnice
                if (y < 0) {
                    return TopLeft.GetCell(x + d, y + d);
                } else {
                    return BotLeft.GetCell(x + d, y - d);
                }
            } else {
                if (y < 0) {
                    return TopRight.GetCell(x - d, y + d);
                } else { 
                    return BotRight.GetCell(x - d, y - d);
                }
            }
        }

        /// <summary>
        /// Výpočet jedné generace dopředu
        /// </summary>
        /// <param name="bitmask"></param>
        /// <returns></returns>
        protected QNode OneGeneration(int bitmask) {
            if (bitmask == 0) {     //nula živých sousedů = smrt
                return Create(false);
            }
            int me = (bitmask >> 5) & 1;    //dostanu binárně informaci o sobě
            int neighbors = 0;
            bitmask &= 0x757; //Hexa maska pro 9 bitů
            while (bitmask != 0) { //počítání jedniček
                neighbors++;
                bitmask &= bitmask - 1;
            }

            if (neighbors == 3 || (neighbors == 2 && me != 0)) {    //podmínky hry
                return Create(true);
            }
            return Create(false);
        }

        /// <summary>
        /// Na procházení malých prostorů, kde se quadtree nedá použít (klasická simulace)
        /// </summary>
        /// <returns></returns>
        protected QNode SlowSimulation() {
            int allbits = 0;
            for (int y = -2; y < 2; y++) {
                for (int x = -2; x < 2; x++) {
                    allbits = (allbits << 1) + Convert.ToInt32(GetCell(x, y).IsAlive);
                    //nastaví sousední buňky
                }
            }
            return Create(OneGeneration(allbits >> 5), OneGeneration(allbits >> 4),
                OneGeneration(allbits >> 1), OneGeneration(allbits));
                    //výpočet jedné generace dopředu pro každou čtvrtinu buňky
        }

        /// <summary>
        /// Vrátí vycentrovanou podbuňku
        /// </summary>
        /// <returns></returns>
        protected QNode CenteredSubnode() {
            return Create(TopLeft.BotRight, TopRight.BotLeft,
                BotLeft.TopRight, BotRight.TopLeft);
        }

        /// <summary>
        /// Vrátí horizontálně vycentrovanou podbuňku
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        protected QNode CenteredHorizontal(QNode left, QNode right) {
            return Create(left.TopRight.BotRight, right.TopLeft.BotLeft, 
                left.BotRight.TopRight, right.BotLeft.TopLeft);
        }
        /// <summary>
        /// Vrátí vertikálně vycentrovanou podbuňku
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        protected QNode CenteredVertical(QNode left, QNode right) {
            return Create(left.BotLeft.BotRight, left.BotRight.BotLeft, 
                right.TopLeft.TopRight, right.TopRight.TopLeft);
        }
        /// <summary>
        /// Dvojité zanoření
        /// </summary>
        /// <returns></returns>
        protected QNode CenteredSubSubnode() {
            return Create(TopLeft.BotRight.BotRight, TopRight.BotLeft.BotLeft,
                BotLeft.TopRight.TopRight, BotRight.TopLeft.TopLeft);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected QNode NextGen() {
            if (Population == 0) {  //rychle přeskočí prázdné kvadranty
                return TopLeft;
            }
            if (Level == 2) {   //na levelu 2 musíme spočítat další generaci klasicky
                return SlowSimulation();
            }
            //http://www.drdobbs.com/jvm/an-algorithm-for-compressing-space-and-t/184406478?pgno=1
            var n00 = TopLeft.CenteredSubnode();
            var n01 = CenteredHorizontal(TopLeft, TopRight);
            var n02 = TopRight.CenteredSubnode();
            var n10 = CenteredVertical(TopLeft, BotLeft);
            var n11 = CenteredSubSubnode();
            var n12 = CenteredVertical(TopRight, BotRight);
            var n20 = BotLeft.CenteredSubnode();
            var n21 = CenteredHorizontal(BotLeft, BotRight);
            var n22 = BotRight.CenteredSubnode();
            return Create(Create(n00, n01, n10, n11).NextGeneration(),
                Create(n01, n02, n11, n12).NextGeneration(),
                Create(n10, n11, n20, n21).NextGeneration(),
                Create(n11, n12, n21, n22).NextGeneration());
        }
        /// <summary>
        /// Vrací buňku z cache
        /// </summary>
        /// <returns></returns>
        protected QNode Intern() {
            QNode node = null;
            if (cache.TryGetValue(this, out node)) {    //slovník, true - najde, false - nenajde
                return node;
            }
            cache[this] = this;                 //každá jedinečná buňka se v paměti právě jednou
            return this;
        }
        /// <summary>
        /// Veřejná metoda Nextgeneration, která používá cache
        /// </summary>
        /// <returns></returns>
        public QNode NextGeneration() {
            if (CacheResult == null) {   //když je v cachi tak vrátím, když ne, vypočítám
                CacheResult = NextGen();
            }
            return CacheResult;
        }
        /// <summary>
        /// Umožňuje říci, zda jsou dvě buňky stejné, aby se nemuselo iterovat, přes jejich zanoření (pomocí hashcodu)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            QNode node = (QNode)obj;
            if (node.Level != Level) { //nemůžou být stejné, když je jinej level
                return false;
            }
            if (IsLeaf) {   //buď obě dvě živé/mrtvé
                return node.IsAlive == IsAlive;
            }               //počítá hash
            return object.Equals(TopLeft, node.TopLeft) && object.Equals(TopRight, node.TopRight) &&
                object.Equals(BotLeft, node.BotLeft) && object.Equals(BotRight, node.BotRight);
        }
        /// <summary>
        /// Spočítá hashcode, přepsání gethashcodu (override)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            if (IsLeaf) {
                return Convert.ToInt32(IsAlive); //vrací jenom 0 nebo 1
            }
            return RuntimeHelpers.GetHashCode(TopLeft) +        //počítáme hashcode pro všechny 4 buňky
                11 * RuntimeHelpers.GetHashCode(TopRight) +     //stejný objekt vrátí stejné číslo
                101 * RuntimeHelpers.GetHashCode(BotLeft) +
                1007 * RuntimeHelpers.GetHashCode(BotRight);
        }

    }
}

