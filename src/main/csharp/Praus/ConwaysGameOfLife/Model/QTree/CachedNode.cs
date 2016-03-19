using System;


namespace Praus.ConwaysGameOfLife.Model.QTree {
    public class CachedNode : NormalNode, INode {

        public CachedNode(INode northWest, INode northEast, INode southWest, INode southEast)
            : base(northWest, northEast, southWest, southEast){
        }

        public CachedNode(bool alive) 
            : base(alive) {
        }

        public override INode NextGeneration() {
            if (cacheResult == null)
                cacheResult = base.NextGeneration();
            return cacheResult;
        }
        public INode cacheResult = null ;

        public static new Node create() {
            return ((CachedNode)new CachedNode(false).CreateEmptyTree(3)).Internal();
        }

        public override INode Create(bool alive) {
            return new CachedNode(alive).Internal();
        }

        public override INode Create(INode northWest, INode northEast, INode southWest, INode southEast) {
            return new CachedNode(northWest, northEast, southWest, southEast).Internal();
        }
    }
}

