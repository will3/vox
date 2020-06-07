using System.Collections.Generic;
using FarmVox.Scripts.GPU.Shaders;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    internal static class MeshBuilder
    {
        public static MeshResult Build(IEnumerable<Quad> quads,
            IReadOnlyDictionary<Vector3Int, float> waterfallData = null)
        {
            var vertices = new List<Vector3>();
            var colors = new List<Color>();
            var uvs = new List<Vector2>();
            var indices = new List<int>();
            var voxelData = new List<VoxelData>();

            foreach (var quad in quads)
            {
                var index = vertices.Count;

                vertices.Add(quad.a);
                vertices.Add(quad.b);
                vertices.Add(quad.c);
                vertices.Add(quad.d);

                colors.Add(quad.color * quad.ao[0]);
                colors.Add(quad.color * quad.ao[1]);
                colors.Add(quad.color * quad.ao[2]);
                colors.Add(quad.color * quad.ao[3]);

                var waterfall = waterfallData == null
                    ? 0
                    : waterfallData.TryGetValue(quad.coord, out var value)
                        ? value
                        : 0;

                var voxelDataIndex = voxelData.Count;
                uvs.Add(new Vector2(voxelDataIndex, waterfall));
                uvs.Add(new Vector2(voxelDataIndex, waterfall));
                uvs.Add(new Vector2(voxelDataIndex, waterfall));
                uvs.Add(new Vector2(voxelDataIndex, waterfall));

                indices.Add(index);
                indices.Add(index + 1);
                indices.Add(index + 2);
                indices.Add(index + 2);
                indices.Add(index + 3);
                indices.Add(index);

                voxelData.Add(new VoxelData
                {
                    Coord = quad.coord,
                    Normal = quad.normal
                });
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices, 0);
            mesh.SetColors(colors);
            mesh.uv = uvs.ToArray();

            return new MeshResult(voxelData, mesh);
        }
    }
}