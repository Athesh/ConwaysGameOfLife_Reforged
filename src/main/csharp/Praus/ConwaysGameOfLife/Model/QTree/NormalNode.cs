using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class NormalNode : Node, INode{
        public NormalNode(INode northWest, INode northEast, INode southWest, INode southEast)
            : base(northWest, northEast, southWest, southEast){
        }

        public NormalNode(bool alive) 
            : base(alive) {
        }

        public override INode Create(bool alive) {
            return new NormalNode(alive).Internal();
        }

        public override INode Create(INode northWest, INode northEast, INode southWest, INode southEast) {
            return new NormalNode(northWest, northEast, southWest, southEast).Internal();
        }

        public override bool Equals(object obj) {
            Node node = (Node)obj;
            if (node.Level != Level) {
                return false;
            }
            if (IsLeaf) {
                return node.IsAlive == IsAlive;
            }
            return NorthWest.Equals(node.NorthWest) && NorthEast.Equals(node.NorthEast) &&
                SouthWest.Equals(node.SouthWest) && SouthEast.Equals(node.SouthEast);
        }

        public override int GetHashCode() {
            if (IsLeaf) {
                return Convert.ToInt32(IsAlive);
            }
            return RuntimeHelpers.GetHashCode(NorthWest) +
                11 * RuntimeHelpers.GetHashCode(NorthEast) +
                101 * RuntimeHelpers.GetHashCode(SouthWest) +
                1007 * RuntimeHelpers.GetHashCode(SouthEast);
        }

        public Node Internal() {
            Node node = null;
            dictionary.TryGetValue(this, out node);
            if (node != null)
                return node;
            dictionary[this] = this;
            return this;
        }

        public static Dictionary<Node, Node> dictionary = new Dictionary<Node, Node>() ;

        public static Node create() {
            return ((NormalNode)new NormalNode(false).CreateEmptyTree(3)).Internal();
        }
    }
}

