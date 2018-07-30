using UnityEngine;
using UnityEngine.AI;

namespace FarmVox
{
    public class RemoveTreeTask : Task
    {
        Tree tree;

        public RemoveTreeTask(Tree tree)
        {
            this.tree = tree;
        }

        public override void Perform(Actor actor)
        {
            var terrian = Finder.FindTerrian();

            foreach(var coord in tree.trunkCoords) {
                terrian.TreeLayer.Set(coord, 0);
                tree.removedTrunk = true;
            }

            // TODO
            //terrian.TreeMap.RemoveTree(tree);
            done = true;
        }

        public override Vector3Int GetCoord()
        {
            return tree.pivot;
        }
    }

    public class DigTask : Task
    {
        readonly Vector3Int coord;
        readonly Vector3 positionUp;

        public DigTask(Vector3Int coord) 
        {
            this.coord = coord;
            positionUp = coord + new Vector3(0.5f, 1.5f, 0.5f);
        }

        public float digAmount = 1.0f;

        public override void Perform(Actor actor)
        {
            if (done) {
                return;
            }

            //actor.SetDestination(positionUp, 2.0f);

            //if (!actor.ArrivedDestination()) {
            //    // Moving

            //    return;
            //}

            digAmount -= actor.digSpeed;

            if (digAmount <= 0.0f) {
                Finder.FindTerrian().DefaultLayer.Set(coord, 0);
                done = true;
            }
        }

        public override Vector3Int GetCoord()
        {
            return coord;
        }
    }
}