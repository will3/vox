using UnityEngine;

namespace FarmVox.Scripts
{
    public static class Vectors
    {
        public static Vector3Int GetVector3Int(int i, int j, int k, int d)
        {
            switch (d)
            {
                case 0:
                    return new Vector3Int(i, j, k);
                case 1:
                    return new Vector3Int(k, i, j);
                default:
                    return new Vector3Int(j, k, i);
            }
        }

        public static Vector3Int FloorToInt(Vector3 vector) {
            return new Vector3Int(
                Mathf.FloorToInt(vector.x),
                Mathf.FloorToInt(vector.y),
                Mathf.FloorToInt(vector.z));
        }

        public static Vector2Int GetXZ(Vector3Int vector) {
            return new Vector2Int(vector.x, vector.z);
        }
    }
}