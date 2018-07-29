using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class Cube
    {
        static Mesh[] meshes;
        static Mesh cubeMesh;

        public static Mesh CubeMesh
        {
            get
            {
                return cubeMesh;
            }
        }

        static Cube()
        {
            meshes = new Mesh[] {
                GenQuad(0, false),
                GenQuad(0, true),
                GenQuad(1, false),
                GenQuad(1, true),
                GenQuad(2, false),
                GenQuad(2, true)
            };

            cubeMesh = GenCube();
        }

        public static Mesh GetQuad(int d, bool front)
        {
            var index = d * 2 + (front ? 1 : 0);
            return meshes[index];
        }

        static Mesh GenCube() {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvs = new List<Vector2>();

            GenQuad(0, false, vertices, indices, uvs);
            GenQuad(0, true, vertices, indices, uvs);
            GenQuad(1, false, vertices, indices, uvs);
            GenQuad(1, true, vertices, indices, uvs);
            GenQuad(2, false, vertices, indices, uvs);
            GenQuad(2, true, vertices, indices, uvs);

            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices.ToArray(), 0);
            mesh.uv = uvs.ToArray();

            return mesh;  
        }

        static Mesh GenQuad(int d, bool front)
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvs = new List<Vector2>();
            GenQuad(d, front, vertices, indices, uvs);
            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices.ToArray(), 0);

            return mesh;
        }

        static void GenQuad(int d, bool front, List<Vector3> vertices, List<int> indices, List<Vector2> uvs) {
            var u = (d + 1) % 3;
            var v = (d + 2) % 3;

            var diffI = front ? 1.0f : 0.0f;

            var v0 = GetVector(diffI, 0, 0, d);
            var v1 = GetVector(diffI, 1.0f, 0, d);
            var v2 = GetVector(diffI, 1.0f, 1.0f, d);
            var v3 = GetVector(diffI, 0, 1.0f, d);

            int index = vertices.Count;

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));

            if (front) {
                indices.Add(index + 0);
                indices.Add(index + 1);
                indices.Add(index + 2);
                indices.Add(index + 2);
                indices.Add(index + 3);
                indices.Add(index + 0);
            } else {
                indices.Add(index + 2);
                indices.Add(index + 1);
                indices.Add(index + 0);
                indices.Add(index + 0);
                indices.Add(index + 3);
                indices.Add(index + 2);
            }
        }

        static Vector3 GetVector(float i, float j, float k, int d)
        {
            if (d == 0)
            {
                return new Vector3(i, j, k);
            }
            if (d == 1)
            {
                return new Vector3(k, i, j);
            }
            return new Vector3(j, k, i);
        }
    }
}