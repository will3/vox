using UnityEngine;

namespace FarmVox
{
    public abstract class Task
    {
        public Designation designation;
        public float priority;
        public bool done;
        public abstract void Perform(Actor actor);
        public abstract Vector3Int GetCoord();
    }
}