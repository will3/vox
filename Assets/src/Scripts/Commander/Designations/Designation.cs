using UnityEngine;

namespace FarmVox
{

    public abstract class Designation
    {
        public GameObject designationObject;
        public Box box;
        public BoundsInt bounds;

        public virtual void Start() { }
        public abstract void Update();

        protected void CreateBox() {
            if (designationObject == null) {
                designationObject = new GameObject("designation");
                box = designationObject.AddComponent<Box>();
                box.SetBounds(bounds);
                box.transform.parent = Commander.Instance.transform;
            }
        }

        protected virtual string GetName() {
            return "designation";
        }

        protected void AdjustBounds()
        {
            var start = bounds.min;
            var end = bounds.max;

            var y = end.y;

            // Update y bounds;

            int maxY = end.y;
            int minY = start.y;

            for (var x = start.x; x <= end.x; x++)
            {
                for (var z = start.z; z <= end.z; z++)
                {
                    var result = FindGroundCoord(x, y, z);
                    if (result != null)
                    {
                        var coord = result.GetCoord();

                        if (coord.y > maxY)
                        {
                            maxY = coord.y;
                        }

                        if (coord.y < minY)
                        {
                            minY = coord.y;
                        }
                    }
                }
            }

            start.y = minY;
            end.y = maxY + 2;

            // Adjust bounds
            bounds.min = start;
            bounds.max = end;
        }

        RaycastResult FindGroundCoord(int x, int y, int z)
        {
            var yOffset = 2;
            var maxTries = 5;

            for (var i = 0; i < maxTries; i++)
            {
                var result = FindGroundCoord(x, y, z, yOffset);
                if (result != null)
                {
                    return result;
                }
                yOffset *= 2;
            }

            return null;
        }

        RaycastResult FindGroundCoord(int x, int y, int z, int yOffset)
        {
            var coord = new Vector3Int(x, y + yOffset, z);
            var ray = new Ray(coord + new Vector3(0.5f, 0.5f, 0.5f), Vector3.down);

            var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

            return result;
        }
    }
}