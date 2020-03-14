using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    public struct Quad
    {
        public Vector3 A;
            
        public Vector3 B;
            
        public Vector3 C;

        public Vector3 D;
            
        public Color Color;

        public Vector4 AO;
            
        public int X;
            
        public int Y;
            
        public int Z;

        public int GetIndex(int size)
        {
            return X * size * size + Y * size + Z;
        }

        public Vector3Int Coord
        {
            get { return new Vector3Int(X, Y, Z); }
        }

        public static int Size
        {
            get
            {
                return
                    sizeof(float) * 3 * 4 +
                    sizeof(float) * 4 +
                    sizeof(float) * 4 +
                    sizeof(int) * 3;
            }
        }
    }
}