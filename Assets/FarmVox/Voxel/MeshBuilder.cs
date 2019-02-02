using System.Collections.Generic;
using FarmVox.GPU.Shaders;
using UnityEngine;

namespace FarmVox.Voxel
{
    internal class MeshBuilder
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly List<int> _indices = new List<int>();

        private void AddTriangle(MesherGpu.Triangle triangle)
        {
            var i = _vertices.Count / 3;

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

        public MeshBuilder AddTriangles(IEnumerable<MesherGpu.Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                AddTriangle(triangle);
            }

            return this;
        }

        public Mesh Build()
        {
            var mesh = new Mesh();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_indices, 0);
            mesh.SetColors(_colors);
            mesh.uv = _uvs.ToArray();
            return mesh;
        }
    }
}