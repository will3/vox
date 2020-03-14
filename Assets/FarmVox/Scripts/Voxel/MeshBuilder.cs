using System.Collections.Generic;
using FarmVox.Scripts;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Voxel
{
    internal class MeshBuilder
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly List<int> _indices = new List<int>();
        private readonly List<VoxelData> _voxelData = new List<VoxelData>();

        private void AddTriangle(Quad quad, Dictionary<Vector3Int, float> waterfallData)
        {
            var index = _vertices.Count;

            _vertices.Add(quad.A);
            _vertices.Add(quad.B);
            _vertices.Add(quad.C);
            _vertices.Add(quad.D);

            _colors.Add(quad.Color * quad.AO[0]);
            _colors.Add(quad.Color * quad.AO[1]);
            _colors.Add(quad.Color * quad.AO[2]);
            _colors.Add(quad.Color * quad.AO[3]);

            var waterfall = waterfallData == null
                ? 0
                : waterfallData.TryGetValue(quad.Coord, out var value)
                    ? value
                    : 0;

            var voxelDataIndex = _voxelData.Count;
            _uvs.Add(new Vector2(voxelDataIndex, waterfall));
            _uvs.Add(new Vector2(voxelDataIndex, waterfall));
            _uvs.Add(new Vector2(voxelDataIndex, waterfall));
            _uvs.Add(new Vector2(voxelDataIndex, waterfall));

            _indices.Add(index);
            _indices.Add(index + 1);
            _indices.Add(index + 2);
            _indices.Add(index + 2);
            _indices.Add(index + 3);
            _indices.Add(index);

            _voxelData.Add(new VoxelData
            {
                Coord = quad.Coord,
                Normal = quad.Normal
            });
        }

        public MeshBuilder AddTriangles(IEnumerable<Quad> triangles,
            Dictionary<Vector3Int, float> waterfallData = null)
        {
            foreach (var triangle in triangles)
            {
                AddTriangle(triangle, waterfallData);
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