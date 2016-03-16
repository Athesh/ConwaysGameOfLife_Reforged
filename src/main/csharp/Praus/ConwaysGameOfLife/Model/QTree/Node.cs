using System;

namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class Node : NodeBaseAbstract {

        public Node(INode northWest, INode northEast, INode southWest, INode southEast)
            : base(northWest, northEast, southWest, southEast){
        }

        public Node(bool alive) 
            : base(alive) {
        }

        public override INode Create(bool alive) {
            return new Node(alive);
        }

        public override INode Create(INode northWest, INode northEast, INode southWest, INode southEast) {
            return new Node(northWest, northEast, southWest, southEast);
        }
    }
}

