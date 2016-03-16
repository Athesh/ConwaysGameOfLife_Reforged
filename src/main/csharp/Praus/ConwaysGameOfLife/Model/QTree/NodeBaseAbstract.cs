using System;
using System.Linq;
using Praus.ConwaysGameOfLife.Utils;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    /// <summary>
    /// Node base abstract class.
    /// Hold generic logic behind all node subclasses.
    /// Generaly immutable.
    /// </summary>
    public abstract class NodeBaseAbstract : INode {
        
        public INode NorthWest { get; }
        public INode NorthEast { get; }
        public INode SouthWest { get; }
        public INode SouthEast { get; }
        public uint Level { get; }
        public ulong Population { get; }
        public bool IsAlive { get; }
        public bool IsLeaf { get; }

        public virtual INode GetLeaf(int x, int y) {
            if (Level == 0) {
                return this;
            }
            uint distance = (uint)Math.Pow(2, Level - 1);
            if (x < 0) {
                if (y < 0) {
                    return NorthWest.GetLeaf(x + distance, y + distance);
                } else {
                    return SouthWest.GetLeaf(x + distance, y - distance);
                }
            } else {
                if (y < 0) {
                    return NorthEast.GetLeaf(x - distance, y + distance);
                } else { 
                    return SouthEast.GetLeaf(x - distance, y - distance);
                }
            }
        }

        public virtual INode SetLeaf(int x, int y) {
            if (Level == 0) {
                return Create(true);
            }
            uint distance = (uint)Math.Pow(2, Level - 1);
            if (x < 0) {
                if (y > 0) {
                    return Create(NorthWest.SetLeaf(x, y), NorthEast, SouthWest, SouthEast);
                } else {
                    return Create(NorthWest, NorthEast, SouthWest.SetLeaf(x, y), SouthEast);
                }
            } else {
                if (y < 0) {
                    return Create(NorthWest, NorthEast.SetLeaf(x, y), SouthWest, SouthEast);
                } else {
                    return Create(NorthWest, NorthEast, SouthWest, SouthEast.SetLeaf(x, y));
                }
            }
        }

        public virtual INode ExpandTree() {
            var emptySpace = CreateEmptyTree(Level - 1);
            return Create(
                Create(emptySpace, emptySpace, emptySpace, NorthWest),
                Create(emptySpace, emptySpace, NorthEast, emptySpace),
                Create(emptySpace, SouthWest, emptySpace, emptySpace),
                Create(SouthEast, emptySpace, emptySpace, emptySpace)
            );

        }

        public virtual INode CreateEmptyTree(uint level) {
            if (level == 0) {
                return Create(false);
            }
            var node = CreateEmptyTree(level - 1);
            return Create(node, node, node, node);
        }

        public abstract INode Create(bool alive);

        public abstract INode Create(INode northWest, INode northEast, INode southWest, INode southEast);
        /// <summary>
        /// Create the instance of NodeBaseAbstract class. 
        /// All subnodes have to be on same level and cannot be null.  
        /// </summary>
        /// <param name="northWest">North west node</param>
        /// <param name="northEast">North east node</param>
        /// <param name="southWest">South west node</param>
        /// <param name="southEast">South east node</param>
        public NodeBaseAbstract(INode northWest, INode northEast, INode southWest, INode southEast) {
            INode[] nodes = { northWest, northEast, southEast, southWest };
            if (nodes.Any(node => node == null)) {
                throw new ArgumentNullException($"Any of these {nameof(northWest)}, {nameof(northEast)}, " +
                    $"{nameof(southWest)}, {nameof(southEast)}.", "Any of the given nodes cannot be null!");
            } else if (nodes.Any(node => node.Level != northWest.Level)) {
                throw new ArgumentException("The given nodes do not have the same RootDistance!");
            }

            NorthEast = northEast;
            NorthWest = northWest;
            SouthEast = southEast;
            SouthWest = southWest;

            Level = northWest.Level + 1u;
            Population = nodes.Select(node => node.Population).Sum();

            IsLeaf = false;
            IsAlive = Population > 0;
        }

        /// <summary>
        /// Create new instance of NodeBaseAbstract class in "Leaf" mode.
        /// Leaf can be either dead or alive.
        /// </summary>
        /// <param name="alive">If set to true the "Leaf" is alive</param>
        public NodeBaseAbstract(bool alive) {
            NorthEast = null;
            NorthWest = null;
            SouthEast = null;
            SouthWest = null;

            Level = 0;
            Population = alive ? 1UL : 0UL;

            IsLeaf = true;
            IsAlive = alive;
        }
    }
}

