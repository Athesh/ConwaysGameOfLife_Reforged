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

        Node slowSimulation() {
            int allbits = 0 ;
            for (int y=-2; y<2; y++)
                for (int x=-2; x<2; x++)
                    allbits = (allbits << 1) + getBit(x, y) ;
            return Create(OneGeneration(allbits>>5), OneGeneration(allbits>>4),
                OneGeneration(allbits>>1), OneGeneration(allbits)) ;
        }

        Node centeredSubnode() {
            return Create(NorthWest.SouthEast, NorthEast.SouthWest, 
                SouthWest.NorthEast, SouthEast.NorthWest);
        }

        Node centeredHorizontal(Node west, Node east) {
            return Create(west.NorthEast.SouthEast, east.NorthWest.SouthWest, 
                west.SouthEast.NorthEast, east.SouthWest.NorthWest);
        }

        Node centeredVertical(Node n, Node s) {
            return Create(n.SouthWest.SouthEast, n.SouthEast.SouthWest, 
                s.NorthWest.NorthEast, s.NorthEast.NorthWest);
        }

        Node centeredSubSubnode() {
            return Create(NorthWest.SouthEast.SouthEast, NorthEast.SouthWest.SouthWest, 
                SouthWest.NorthEast.NorthEast, SouthEast.NorthWest.NorthWest);
        }
    }
}

