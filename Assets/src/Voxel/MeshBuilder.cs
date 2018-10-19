using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    class MeshBuilder
    {
        List<Vector3> vertices = new List<Vector3>();

        public bool colliderMesh = false;

        public List<Vector3> Vertices
        {
            get
            {
                return vertices;
            }
        }

        List<Color> colors = new List<Color>();

        public List<Color> Colors
        {
            get
            {
                return colors;
            }
        }

        List<Vector2> uvs = new List<Vector2>();

        public List<Vector2> Uvs
        {
            get
            {
                return uvs;
            }
        }

        List<int> indices = new List<int>();

        public List<int> Indices
        {
            get
            {
                return indices;
            }
        }

        public void AddTriangle(MesherGpu.Triangle triangle)
        {
            int i = vertices.Count / 3;

            vertices.Add(triangle.A);
            vertices.Add(triangle.B);
            vertices.Add(triangle.C);

            indices.Add(i * 3);
            indices.Add(i * 3 + 1);
            indices.Add(i * 3 + 2);

            if (!colliderMesh) {
                colors.Add(triangle.ColorA);
                colors.Add(triangle.ColorB);
                colors.Add(triangle.ColorC);

                uvs.Add(new Vector2(0, triangle.Index));
                uvs.Add(new Vector2(0, triangle.Index));
                uvs.Add(new Vector2(0, triangle.Index));
            }
        }
    }
}