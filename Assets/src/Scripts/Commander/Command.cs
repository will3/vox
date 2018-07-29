using UnityEngine;

namespace FarmVox
{
    public abstract class Command
    {
        public abstract void Update();
        public Box box;
        public Transform transform;
        public bool done;
    }
}