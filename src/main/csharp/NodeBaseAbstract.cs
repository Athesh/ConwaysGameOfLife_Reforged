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
        public uint RootDistance { get; }
        public ulong Population { get; }
        public bool IsAlive { get; }
        public bool IsLeaf { get; }

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
            } else if (nodes.Any(node => node.RootDistance != northWest.RootDistance)) {
                throw new ArgumentException("The given nodes do not have the same RootDistance!");
            }

            NorthEast = northEast;
            NorthWest = northWest;
            SouthEast = southEast;
            SouthWest = southWest;

            RootDistance = northWest.RootDistance + 1u;
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

            RootDistance = 0;
            Population = alive ? 1UL : 0UL;

            IsLeaf = true;
            IsAlive = alive;
        }
    }
}

