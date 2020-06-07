using System;
using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    [Serializable]
    public struct Quad
    {
        public Vector3 a;
            
        public Vector3 b;
            
        public Vector3 c;

        public Vector3 d;
            
        public Color color;

        public Vector4 ao;
            
        public Vector3Int coord;

        public Vector3 normal;

        public static int Size =>
            sizeof(float) * 3 * 4 +
            sizeof(float) * 4 +
            sizeof(float) * 4 +
            sizeof(int) * 3 + 
            sizeof(float) * 3;
    }
}