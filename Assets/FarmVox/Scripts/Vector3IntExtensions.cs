using UnityEngine;

namespace FarmVox.Scripts
{
    public static class Vector3IntExtensions
    {
        public static Vector3Int GetOrigin(this Vector3Int vector, int size)
        {
            var sizeF = (float) size;
            return new Vector3Int(
                Mathf.FloorToInt(vector.x / sizeF) * size,
                Mathf.FloorToInt(vector.y / sizeF) * size,
                Mathf.FloorToInt(vector.z / sizeF) * size
            );
        }
    }
}