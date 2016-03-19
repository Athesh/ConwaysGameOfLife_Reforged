using System;

namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class Node : NodeBaseAbstract, INode {

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

        public INode OneGeneration(int bitmask) {
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

        public virtual INode NextGeneration() {
            // skip empty regions quickly
            if (Population == 0)
                return NorthWest ;
            if (Level == 2)
                return slowSimulation() ;
            INode n00 = ((Node)NorthWest).centeredSubnode(),
            n01 = centeredHorizontal((Node)NorthWest, (Node)NorthEast),
            n02 = ((Node)NorthEast).centeredSubnode(),
            n10 = centeredVertical((Node)NorthWest, (Node)SouthWest),
            n11 = centeredSubSubnode(),
            n12 = centeredVertical((Node)NorthEast, (Node)SouthEast),
            n20 = ((Node)SouthWest).centeredSubnode(),
            n21 = centeredHorizontal((Node)SouthWest, (Node)SouthEast),
            n22 = ((Node)SouthEast).centeredSubnode() ;
            return Create(((Node)Create(n00, n01, n10, n11)).NextGeneration(),
                ((Node)Create(n01, n02, n11, n12)).NextGeneration(),
                ((Node)Create(n10, n11, n20, n21)).NextGeneration(),
                ((Node)Create(n11, n12, n21, n22)).NextGeneration());
        }

        INode slowSimulation() {
            int allbits = 0 ;
            for (int y = -2; y < 2; y++)
                for (int x = -2; x < 2; x++)
                    allbits = (allbits << 1) + Convert.ToInt32(GetLeaf(x, y).IsAlive);
            return Create(OneGeneration(allbits >> 5), OneGeneration(allbits >> 4),
                OneGeneration(allbits >> 1), OneGeneration(allbits));
        }

        INode centeredSubnode() {
            return Create(NorthWest.SouthEast, NorthEast.SouthWest, 
                SouthWest.NorthEast, SouthEast.NorthWest);
        }

        INode centeredHorizontal(Node west, Node east) {
            return Create(west.NorthEast.SouthEast, east.NorthWest.SouthWest, 
                west.SouthEast.NorthEast, east.SouthWest.NorthWest);
        }

        INode centeredVertical(Node n, Node s) {
            return Create(n.SouthWest.SouthEast, n.SouthEast.SouthWest, 
                s.NorthWest.NorthEast, s.NorthEast.NorthWest);
        }

        INode centeredSubSubnode() {
            return Create(NorthWest.SouthEast.SouthEast, NorthEast.SouthWest.SouthWest, 
                SouthWest.NorthEast.NorthEast, SouthEast.NorthWest.NorthWest);
        }
    }
}

