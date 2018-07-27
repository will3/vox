using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    class VoxelMeshBuilder
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

        public void AddTriangle(MesherGPU.Triangle triangle)
        {
            int i = vertices.Count / 3;

            vertices.Add(triangle.a);
            vertices.Add(triangle.b);
            vertices.Add(triangle.c);

            indices.Add(i * 3);
            indices.Add(i * 3 + 1);
            indices.Add(i * 3 + 2);

            if (!colliderMesh) {
                colors.Add(triangle.colorA);
                colors.Add(triangle.colorB);
                colors.Add(triangle.colorC);

                uvs.Add(new Vector2(triangle.waterfall, 0));
                uvs.Add(new Vector2(triangle.waterfall, 0));
                uvs.Add(new Vector2(triangle.waterfall, 0));    
            }
        }
    }
}