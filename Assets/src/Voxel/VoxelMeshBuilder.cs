using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    class VoxelMeshBuilder
    {
        List<Vector3> vertices = new List<Vector3>();

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

        Dictionary<Vector3, int> verticeMap = new Dictionary<Vector3, int>();

        public void AddTriangle(MesherGPU.Triangle triangle)
        {
            int i = vertices.Count / 3;

            vertices.Add(triangle.a);
            vertices.Add(triangle.b);
            vertices.Add(triangle.c);

            indices.Add(i * 3);
            indices.Add(i * 3 + 1);
            indices.Add(i * 3 + 2);

            colors.Add(triangle.colorA);
            colors.Add(triangle.colorB);
            colors.Add(triangle.colorC);

            uvs.Add(new Vector2(triangle.waterfall, 0));
            uvs.Add(new Vector2(triangle.waterfall, 0));
            uvs.Add(new Vector2(triangle.waterfall, 0));
        }

        int AddVertice(Vector3 vector, Color color, float waterfall)
        {
            if (verticeMap.ContainsKey(vector))
            {
                return verticeMap[vector];
            }

            vertices.Add(vector);
            colors.Add(color);
            uvs.Add(new Vector2(waterfall, 0));

            int index = vertices.Count - 1;
            verticeMap[vector] = index;
            return index;
        }
    }
}