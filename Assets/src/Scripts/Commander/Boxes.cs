using UnityEngine;

namespace FarmVox
{
    public static class Boxes
    {
        public static BoundsInt AdjustBounds(BoundsInt bounds)
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

            return bounds;
        }

        static RaycastResult FindGroundCoord(int x, int y, int z)
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

        static RaycastResult FindGroundCoord(int x, int y, int z, int yOffset)
        {
            var coord = new Vector3Int(x, y + yOffset, z);
            var ray = new Ray(coord + new Vector3(0.5f, 0.5f, 0.5f), Vector3.down);

            var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

            return result;
        }
    }
}