using System;
using NUnit.Framework;
using Praus.ConwaysGameOfLife.Model.QTree;

namespace Praus.GameOfLife.Model.Test {
    [TestFixture]
    public class QTreeNodeTest {

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QTreeNodeBaseAbstractTestNullArgTest() {
            new QTreeNodeMock(null, null, null, null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void QTreeNodeBaseAbstractDifferentFloorTest() {
            var aliveNode = new QTreeNodeMock(true);
            var deadNode = new QTreeNodeMock(false);
            var levelOneNode = new QTreeNodeMock(aliveNode, deadNode, aliveNode, deadNode);
            new QTreeNodeMock(levelOneNode, levelOneNode, levelOneNode, deadNode);
        }
        [Test]
        public void QTreeNodeBaseAbstractRootDistanceTest() {
            var aliveNode = new QTreeNodeMock(true);
            var deadNode = new QTreeNodeMock(false);
            var levelOneNode = new QTreeNodeMock(aliveNode, deadNode, aliveNode, deadNode);
            var levelTwoNode = new QTreeNodeMock(levelOneNode, levelOneNode, levelOneNode, levelOneNode);
            var levelThreeNode = new QTreeNodeMock(levelTwoNode, levelTwoNode, levelTwoNode, levelTwoNode);
            Assert.AreEqual(3UL, levelThreeNode.RootDistance);
        }
        [Test]
        public void QTreeNodeBaseAbstractPopulationTest() {
            var aliveNode = new QTreeNodeMock(true);
            var deadNode = new QTreeNodeMock(false);

            var level1NodeP0 = new QTreeNodeMock(deadNode, deadNode, deadNode, deadNode);
            var level1NodeP1 = new QTreeNodeMock(deadNode, deadNode, deadNode, aliveNode);
            var level1NodeP2 = new QTreeNodeMock(deadNode, deadNode, aliveNode, aliveNode);

            var level2NodeP3 = new QTreeNodeMock(level1NodeP0, level1NodeP1, level1NodeP0, level1NodeP2);
            var level2NodeP5 = new QTreeNodeMock(level1NodeP2, level1NodeP1, level1NodeP0, level1NodeP2);
            var level2NodeP0 = new QTreeNodeMock(level1NodeP0, level1NodeP0, level1NodeP0, level1NodeP0);

            var level3NodeP8 = new QTreeNodeMock(level2NodeP0, level2NodeP3, level2NodeP5, level2NodeP0);
            var level3NodeP9 = new QTreeNodeMock(level2NodeP3, level2NodeP3, level2NodeP3, level2NodeP0);

            Assert.AreEqual(8UL, level3NodeP8.Population);
            Assert.AreEqual(9UL, level3NodeP9.Population);
        }
    }
    public class QTreeNodeMock : NodeBaseAbstract {
        public QTreeNodeMock(INode nw, INode ne, INode sw, INode se) : base (nw, ne, sw, se) {
        }
        public QTreeNodeMock(bool alive) : base (alive) {
        }
    }
}

