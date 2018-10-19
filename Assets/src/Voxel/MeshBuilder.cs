using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    internal class MeshBuilder
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<Color> Colors = new List<Color>();
        public readonly List<Vector2> Uvs = new List<Vector2>();
        public readonly List<int> Indices = new List<int>();

        public void AddTriangle(MesherGpu.Triangle triangle)
        {
            var i = Vertices.Count / 3;

            Vertices.Add(triangle.A);
            Vertices.Add(triangle.B);
            Vertices.Add(triangle.C);

            Indices.Add(i * 3);
            Indices.Add(i * 3 + 1);
            Indices.Add(i * 3 + 2);

            Colors.Add(triangle.ColorA);
            Colors.Add(triangle.ColorB);
            Colors.Add(triangle.ColorC);

            Uvs.Add(new Vector2(0, triangle.Index));
            Uvs.Add(new Vector2(0, triangle.Index));
            Uvs.Add(new Vector2(0, triangle.Index));
        }
    }
}