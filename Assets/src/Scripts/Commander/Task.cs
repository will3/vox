using UnityEngine;

namespace FarmVox
{
    public abstract class Task
    {
        public bool done;
        public abstract void Perform(Actor actor);
        public abstract Vector3Int GetCoord();
    }
}