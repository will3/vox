using UnityEngine;

namespace FarmVox
{
    public abstract class Command
    {
        public Commander commander;

        public abstract bool Update();

        protected Box box;
        protected GameObject boxObject;

        protected bool DragBox()
        {
            if (boxObject == null) {
                boxObject = new GameObject("box");
                boxObject.transform.parent = commander.transform;
                box = boxObject.AddComponent<Box>();
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                var result = VoxelRaycast.TraceMouse(1 << UserLayers.terrian);
                if (result != null)
                {
                    box.AddCoord(result.GetCoord());
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                return true;
            }

            return false;
        }

        public void RemoveBox() {
            Object.Destroy(boxObject);
        }
    }
}