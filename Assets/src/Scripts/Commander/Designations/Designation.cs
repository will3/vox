using UnityEngine;

namespace FarmVox
{

    public abstract class Designation
    {
        public GameObject designationObject;
        public HighlightBox box;
        public BoundsInt bounds;

        public Vector3Int center {
            get {
                return Vectors.FloorToInt(bounds.center);
            }
        }

        public virtual void Start() { }
        public abstract void Update();
        public DesignationType type;

        protected void CreateBox() {
            if (designationObject == null) {
                designationObject = new GameObject("designation");
                box = designationObject.AddComponent<HighlightBox>();
                box.SetBounds(bounds);
                box.transform.parent = Commander.Instance.transform;
            }
        }

        protected virtual string GetName() {
            return "designation";
        }
    }
}