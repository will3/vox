using UnityEngine;

namespace FarmVox
{
    public abstract class Task
    {
        public readonly Vector3Int coord;
        public readonly Vector3 positionUp;
        public bool done;

        public Task(Vector3Int coord)
        {
            this.coord = coord;
            positionUp = coord + new Vector3(0.5f, 1.5f, 0.5f);
        }

        public abstract void Perform(Actor actor);
    }
}