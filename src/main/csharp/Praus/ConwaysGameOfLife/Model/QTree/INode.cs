using System;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    /// <summary>
    /// QTree node interface.
    /// </summary>
    public interface INode {
        /// <summary>
        /// Get the north west node(the top right).
        /// </summary>
        /// <value>North west node</value>
        INode NorthWest { get; }
        /// <summary>
        /// Get the north east node(the top left).
        /// </summary>
        /// <value>North east node</value>
        INode NorthEast { get; }
        /// <summary>
        /// Get the south west node(the bottom right).
        /// </summary>
        /// <value>South west node</value>
        INode SouthWest { get; }
        /// <summary>
        /// Get the sout east node(the bottom left).
        /// </summary>
        /// <value>South east node</value>
        INode SouthEast { get; }
        /// <summary>
        /// Get the distance to root node.
        /// </summary>
        /// <value>Distance to the root node</value>
        uint RootDistance { get; }
        /// <summary>
        /// Get the population in this node.
        /// </summary>
        /// <value>Node population</value>
        ulong Population { get; }
        /// <summary>
        /// Get value indicating whether node is "Alive" or "Dead" in case
        /// it is the "Leaf" node. Otherwise throw an exception.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        bool IsAlive { get; }
        /// <summary>
        /// Get value indicating whether node is leaf.
        /// </summary>
        /// <value><c>true</c> if this instance is leaf; otherwise, <c>false</c>.</value>
        bool IsLeaf { get; }
    }
}

