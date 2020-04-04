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
            
        public Vector3Int Coord;

        public Vector3 Normal;

        public static int Size
        {
            get
            {
                return
                    sizeof(float) * 3 * 4 +
                    sizeof(float) * 4 +
                    sizeof(float) * 4 +
                    sizeof(int) * 3 + 
                    sizeof(float) * 3;
            }
        }
    }
}