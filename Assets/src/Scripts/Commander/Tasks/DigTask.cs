using UnityEngine;
using UnityEngine.AI;

namespace FarmVox
{
    public class DigTask : Task
    {
        readonly Vector3Int coord;
        readonly Vector3 positionUp;

        public DigTask(Vector3Int coord) 
        {
            this.coord = coord;
            positionUp = coord + new Vector3(0.5f, 1.5f, 0.5f);
            type = TaskType.Dig;
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
                var chunks = Finder.FindTerrian().DefaultLayer;
                chunks.Set(coord, 0);
                chunks.GetColor(coord);
                var color = chunks.GetColor(coord);
                actor.AddItem(new BlockItem(color));
                done = true;
            }
        }

        public override Vector3Int GetCoord()
        {
            return coord;
        }
    }
}