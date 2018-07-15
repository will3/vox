using UnityEngine;

namespace FarmVox
{
    public static class Cube
    {
        private static Mesh[] meshes;

        static Cube()
        {
            meshes = new Mesh[] {
                genQuad(0, false),
                genQuad(0, true),
                genQuad(1, false),
                genQuad(1, true),
                genQuad(2, false),
                genQuad(2, true)
            };
        }

        public static Mesh GetQuad(int d, bool front)
        {
            var index = d * 2 + (front ? 1 : 0);
            return meshes[index];
        }

        private static Mesh genQuad(int d, bool front)
        {
            var u = (d + 1) % 3;
            var v = (d + 2) % 3;

            var diffI = front ? 1.0f : 0.0f;

            var v0 = getVector(diffI, 0, 0, d);
            var v1 = getVector(diffI, 1.0f, 0, d);
            var v2 = getVector(diffI, 1.0f, 1.0f, d);
            var v3 = getVector(diffI, 0, 1.0f, d);

            var mesh = new Mesh();
            mesh.vertices = new Vector3[] { v0, v1, v2, v3 };
            var triangles = 
                front ? new int[] { 0, 1, 2, 2, 3, 0 } : new int[] { 2, 1, 0, 0, 3, 2 };
            mesh.SetTriangles(triangles, 0);
            return mesh;
        }

        private static Vector3 getVector(float i, float j, float k, int d)
        {
            if (d == 0)
            {
                return new Vector3(i, j, k);
            }
            else if (d == 1)
            {
                return new Vector3(k, i, j);
            }
            return new Vector3(j, k, i);
        }
    }
}