using UnityEngine;

namespace FarmVox.Objects
{
    public class WallObject : IPlaceableObject
    {
        private readonly int _height;

        public WallObject(int height)
        {
            _height = height;
        }

        public Vector3Int GetSize()
        {
            return new Vector3Int(6 ,_height ,6);
        }

        public float GetValue(Vector3Int coord)
        {
            if (coord.x > 0 && coord.x < 5 && coord.z > 0 && coord.z < 5 && coord.y < _height)
            {
                return 1;
            }

            return 0;
        }

        public Color GetColor(Vector3Int coord)
        {
            return new Color(0.2f, 0.2f, 0.2f, 1.0f);
        }
    }
}