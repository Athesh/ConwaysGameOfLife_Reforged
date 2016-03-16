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

        public Node OneGeneration(int bitmask) {
            if (bitmask == 0) {
                return Create(false);
            }
            int self = (bitmask >> 5) & 1;
            bitmask &= 0x757;
            int neighborCount = 0;
            while (bitmask != 0) {
                neighborCount++;
                bitmask &= bitmask - 1;
            }
            if (neighborCount == 3 || (neighborCount == 2 && self != 0)) {
                return Create(true);
            } else {
                return Create(false);
            }
        }
    }
}

