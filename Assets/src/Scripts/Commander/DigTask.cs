using UnityEngine;
using UnityEngine.AI;

namespace FarmVox
{
    public class DigTask : Task
    {
        public DigTask(Vector3Int coord) : base(coord)
        {

        }

        public float digAmount = 1.0f;

        public override void Perform(Actor actor)
        {
            if (done) {
                return;
            }

            actor.SetDestination(positionUp, 2.0f);

            if (!actor.ArrivedDestination()) {
                // Moving

                return;
            }

            digAmount -= actor.digSpeed;

            if (digAmount <= 0.0f) {
                Finder.FindTerrian().DefaultLayer.Set(coord, 0);
                done = true;
            }
        }
    }
}