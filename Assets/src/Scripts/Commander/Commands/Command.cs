using UnityEngine;

namespace FarmVox
{
    public abstract class Command
    {
        public Commander commander;

        public abstract bool Update();

        protected HighlightBox box;
        protected GameObject boxObject;


        protected bool DragBox()
        {
            if (boxObject == null) {
                boxObject = new GameObject("box");
                boxObject.transform.parent = commander.transform;
                box = boxObject.AddComponent<HighlightBox>();
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

        protected void DragLine() {
            
        }

        public void RemoveBox() {
            Object.Destroy(boxObject);
        }
    }
}