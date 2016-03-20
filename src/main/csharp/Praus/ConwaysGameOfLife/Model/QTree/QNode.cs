using System;
using System.Linq;
using Praus.ConwaysGameOfLife.Utils;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class QNode {
        public QNode TopLeft { get; }
        public QNode TopRight { get; }
        public QNode BotLeft { get; }
        public QNode BotRight { get; }
        public int Level { get; }
        public int Population { get; }
        public bool IsLeaf { get; }
        public bool IsAlive { get; }

        private static Dictionary<QNode, QNode> dict { get; } = new Dictionary<QNode, QNode>();
        private QNode CacheResult { get; set; }

        protected QNode(QNode topLeft, QNode topRight, QNode botLeft, QNode botRight) {
            QNode[] nodes = { topLeft, topRight, botLeft, botRight };
            if (nodes.Any(node => node == null)) {
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

        protected QNode(bool alive) {
            TopLeft = TopRight = BotLeft = BotRight = null;
            Level = 0;
            Population = alive ? 1 : 0;
        }

        public static QNode Create() {
            return new QNode(false).CreateEmptyTree(3);
        }

        public QNode Create(QNode TopLeft, QNode TopRight, QNode BotLeft, QNode BotRight) {
            return new QNode(TopLeft, TopRight, BotLeft, BotRight).Intern();
        }

        public QNode Create(bool alive) {
            return new QNode(alive).Intern();
        }

        public QNode CreateEmptyTree(int level) {
            if (level == 0) {
                return Create(false);
            }
            var emptyNode = CreateEmptyTree(level - 1);
            return Create(emptyNode, emptyNode, emptyNode, emptyNode);
        }

        public QNode ExpandTree() {
            var emptySpace = CreateEmptyTree(Level - 1);
            return Create(
                Create(emptySpace, emptySpace, emptySpace, TopLeft),
                Create(emptySpace, emptySpace, TopRight, emptySpace),
                Create(emptySpace, BotLeft, emptySpace, emptySpace),
                Create(BotRight, emptySpace, emptySpace, emptySpace)
            );
        }

        public QNode SetCell(int x, int y) {
            if (Level == 0) {
                return Create(true);
            }
            int d = Convert.ToInt32(Math.Pow(2, Level - 2));
            if (x < 0) {
                if (y > 0) {
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

        public QNode GetCell(int x, int y) {
            if (Level == 0) {
                return this;
            }
            int d = Convert.ToInt32(Math.Pow(2, Level - 2));
            if (x < 0) {
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

        protected QNode OneGeneration(int bitmask) {
            if (bitmask == 0) {
                return Create(false);
            }
            int me = (bitmask << 5) & 1;
            int neighbors = 0;
            bitmask &= 0x757;
            while (bitmask != 0) {
                neighbors++;
                bitmask &= bitmask - 1;
            }

            if (neighbors == 3 || (neighbors == 2 && me != 0)) {
                return Create(true);
            }
            return Create(false);
        }

        protected QNode SlowSimulation() {
            int allbits = 0;
            for (int y = -2; y < 2; y++) {
                for (int x = -2; x < 2; x++) {
                    allbits = (allbits << 1) + Convert.ToInt32(GetCell(x, y).IsAlive);
                }
            }
            return Create(OneGeneration(allbits >> 5), OneGeneration(allbits >> 4),
                OneGeneration(allbits >> 1), OneGeneration(allbits));
        }

        protected QNode CenteredSubnode() {
            return Create(TopLeft.BotRight, TopRight.BotLeft,
                BotLeft.TopRight, BotRight.TopLeft);
        }

        protected QNode CenteredHorizontal(QNode left, QNode right) {
            return Create(left.TopRight.BotRight, right.TopLeft.BotLeft, 
                left.BotRight.TopRight, right.BotLeft.TopLeft);
        }

        protected QNode CenteredVertical(QNode left, QNode right) {
            return Create(left.BotLeft.BotRight, left.BotRight.BotLeft, 
                right.TopLeft.TopRight, right.TopRight.TopLeft);
        }

        protected QNode CenteredSubSubnode() {
            return Create(TopLeft.BotRight.BotRight, TopRight.BotLeft.BotLeft, 
                BotLeft.TopRight.TopRight, BotRight.TopLeft.TopLeft);
        }

        protected QNode NextGen() {
            if (Population == 0) {
                return TopLeft;
            }
            if (Level == 2) {
                return SlowSimulation();
            }
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

        protected QNode Intern() {
            QNode node = null;
            if (dict.TryGetValue(this, out node)) {
                return node;
            }
            dict[this] = this;
            return this;
        }

        public QNode NextGeneration() {
            if (CacheResult == null) {
                CacheResult = NextGen();
            }
            return CacheResult;
        }

        public override bool Equals(object obj) {
            QNode node = (QNode)obj;
            if (node.Level != Level) {
                return false;
            }
            if (IsLeaf) {
                return node.IsAlive == IsAlive;
            }
            return TopLeft.Equals(node.TopLeft) && TopRight.Equals(node.TopRight) &&
                BotLeft.Equals(node.BotLeft) && BotRight.Equals(node.BotRight);
        }

        public override int GetHashCode() {
            if (IsLeaf) {
                return Convert.ToInt32(IsAlive);
            }
            return RuntimeHelpers.GetHashCode(TopLeft) +
                11 * RuntimeHelpers.GetHashCode(TopRight) +
                101 * RuntimeHelpers.GetHashCode(BotLeft) +
                1007 * RuntimeHelpers.GetHashCode(BotRight);
        }

    }
}

