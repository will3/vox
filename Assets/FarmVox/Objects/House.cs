using UnityEngine;

namespace FarmVox.Objects
{
    public class House
    {
        public Vector3Int GetSize()
        {
            return new Vector3Int(5, 6, 5);    
        }

        public float GetValue(Vector3Int coord)
        {
            var size = GetSize();
            var fromTop = size.y - coord.y - 1;
            var fromLeftOrRight = Mathf.Min(coord.x, size.x - coord.x - 1);
            if (fromTop + fromLeftOrRight < 2)
            {
                return 0;
            }

            return 1;
        }

        public Color GetColor(Vector3Int coord)
        {
            return Color.white;
        }
    }
}