using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    internal class MeshBuilder
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();

        public List<Vector3> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        readonly List<Color> _colors = new List<Color>();

        public List<Color> Colors
        {
            get
            {
                return _colors;
            }
        }

        readonly List<Vector2> _uvs = new List<Vector2>();

        public List<Vector2> Uvs
        {
            get
            {
                return _uvs;
            }
        }

        readonly List<int> _indices = new List<int>();

        public List<int> Indices
        {
            get
            {
                return _indices;
            }
        }

        public void AddTriangle(MesherGpu.Triangle triangle)
        {
            int i = _vertices.Count / 3;

            _vertices.Add(triangle.A);
            _vertices.Add(triangle.B);
            _vertices.Add(triangle.C);

            _indices.Add(i * 3);
            _indices.Add(i * 3 + 1);
            _indices.Add(i * 3 + 2);

            _colors.Add(triangle.ColorA);
            _colors.Add(triangle.ColorB);
            _colors.Add(triangle.ColorC);

            _uvs.Add(new Vector2(0, triangle.Index));
            _uvs.Add(new Vector2(0, triangle.Index));
            _uvs.Add(new Vector2(0, triangle.Index));
        }
    }
}