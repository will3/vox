using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Objects
{
    public static class ObjectPlacer
    {
        public static void Place(
            Chunks chunks,
            IPlaceableObject obj,
            Vector3Int offset,
            int rotate)
        {
            var size = obj.GetSize();

            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    for (var k = 0; k < size.z; k++)
                    {
                        var coord = new Vector3Int(i, j, k);
                        var worldCoord = coord + offset;

                        if (rotate > 0)
                        {
                            coord = Rotate(size, coord, rotate);
                        }

                        var value = obj.GetValue(coord);
                        var color = obj.GetColor(coord);

                        chunks.Set(worldCoord, value);
                        chunks.SetColor(worldCoord, color);
                    }
                }
            }
        }

        private static Vector3Int Rotate(Vector3Int size, Vector3Int coord, int rotate)
        {
            var pivot = new Vector3(size.x / 2.0f, 0, size.z / 2.0f);

            var a = rotate * 90f;

            var p = coord + new Vector3(0.5f, 0.5f, 0.5f);

            p -= pivot;
            p = Quaternion.AngleAxis(a, Vector3.up) * p;
            p += pivot;

            return new Vector3Int(
                Mathf.FloorToInt(p.x),
                Mathf.FloorToInt(p.y),
                Mathf.FloorToInt(p.z));
        }
    }
}