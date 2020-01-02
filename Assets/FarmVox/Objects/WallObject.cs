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
            return new Vector3Int(3 ,_height ,3);
        }

        public float GetValue(Vector3Int coord)
        {
            return 1;
        }

        public Color GetColor(Vector3Int coord)
        {
            return new Color(0.2f, 0.2f, 0.2f, 1.0f);
        }
    }
}