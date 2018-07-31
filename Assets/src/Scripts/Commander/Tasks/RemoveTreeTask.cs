using UnityEngine;

namespace FarmVox
{
    public class RemoveTreeTask : Task
    {
        Tree tree;

        public RemoveTreeTask(Tree tree)
        {
            this.tree = tree;
            type = TaskType.RemoveTree;
        }

        public override void Perform(Actor actor)
        {
            var terrian = Finder.FindTerrian();

            foreach (var coord in tree.trunkCoords)
            {
                terrian.DefaultLayer.Set(coord, 0);
                tree.removedTrunk = true;
            }

            foreach (var coord in tree.stumpCoords)
            {
                terrian.DefaultLayer.Set(coord, 0);
                tree.removedStump = true;
            }

            terrian.TreeMap.RemoveTree(tree);
            done = true;
        }

        public override Vector3Int GetCoord()
        {
            return tree.pivot;
        }
    }
}