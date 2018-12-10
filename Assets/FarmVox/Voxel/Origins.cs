using UnityEngine;

namespace FarmVox.Voxel
{
    public static class Origins
    {
        public static Vector3Int GetOrigin(int i, int j, int k, int size)
        {
            var sizeF = (float)size;
            return new Vector3Int(
                Mathf.FloorToInt(i / sizeF) * size,
                Mathf.FloorToInt(j / sizeF) * size,
                Mathf.FloorToInt(k / sizeF) * size
            );
        }
    }
}
