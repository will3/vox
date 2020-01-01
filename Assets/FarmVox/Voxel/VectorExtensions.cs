using UnityEngine;

namespace FarmVox.Voxel
{
    public static class VectorExtensions
    {
        public static Vector3Int GetOrigin(this Vector3Int vector, int size)
        {
            var sizeF = (float)size;
            return new Vector3Int(
                Mathf.FloorToInt(vector.x / sizeF) * size,
                Mathf.FloorToInt(vector.y / sizeF) * size,
                Mathf.FloorToInt(vector.z / sizeF) * size
            );
        }

        public static Vector3Int CeilToInt(this Vector3 vector)
        {
            return new Vector3Int(
                Mathf.CeilToInt(vector.x),
                Mathf.CeilToInt(vector.y),
                Mathf.CeilToInt(vector.z));
        }

        public static Vector3Int FloorToInt(this Vector3 vector)
        {
            return new Vector3Int(
                Mathf.FloorToInt(vector.x),
                Mathf.FloorToInt(vector.y),
                Mathf.FloorToInt(vector.z));
        } 
    }
}
