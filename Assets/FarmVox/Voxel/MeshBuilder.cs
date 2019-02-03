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
        private readonly List<CoordData> _voxelData = new List<CoordData>();
        
        private void AddTriangle(MesherGpu.Triangle triangle)
        {
            var i = _vertices.Count / 3;

            _vertices.Add(triangle.A);
            _vertices.Add(triangle.B);
            _vertices.Add(triangle.C);

            _indices.Add(i * 3);
            _indices.Add(i * 3 + 1);
            _indices.Add(i * 3 + 2);

            _colors.Add(triangle.Color * triangle.AO[0]);
            _colors.Add(triangle.Color * triangle.AO[1]);
            _colors.Add(triangle.Color * triangle.AO[2]);

            _voxelData.Add(new CoordData
            {
                Coord = triangle.Coord
            });
            var index = _voxelData.Count - 1;
            
            _uvs.Add(new Vector2(index, 0));
            _uvs.Add(new Vector2(index, 0));
            _uvs.Add(new Vector2(index, 0));
            
            i = _vertices.Count / 3;
            
            _vertices.Add(triangle.A);
            _vertices.Add(triangle.C);
            _vertices.Add(triangle.D);

            _indices.Add(i * 3);
            _indices.Add(i * 3 + 1);
            _indices.Add(i * 3 + 2);

            _colors.Add(triangle.Color * triangle.AO[0]);
            _colors.Add(triangle.Color * triangle.AO[2]);
            _colors.Add(triangle.Color * triangle.AO[3]);

            _uvs.Add(new Vector2(index, 0));
            _uvs.Add(new Vector2(index, 0));
            _uvs.Add(new Vector2(index, 0));
        }

        public MeshBuilder AddTriangles(IEnumerable<MesherGpu.Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                AddTriangle(triangle);
            }

            return this;
        }

        public MeshResult Build()
        {
            var mesh = new Mesh();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_indices, 0);
            mesh.SetColors(_colors);
            mesh.uv = _uvs.ToArray();
            
            return new MeshResult(_voxelData, mesh);
        }
    }
}