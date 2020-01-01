using System;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BuildingTileMesher : MonoBehaviour
    {
        public BuildingTile tile;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public Terrian terrian;
        public bool dirty = true;

        private void Update()
        {
            if (!dirty)
            {
                return;
            }

            if (tile.coords == null || tile.coords.Length == 0)
            {
                return;
            }

            var meshBuilder = new CubeMeshBuilder();

            var bounds = BoundsHelper.CalcBounds(tile.coords);
            var size = bounds.size;

            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    for (var k = 0; k < size.z; k++)
                    {
                        var c = bounds.position + new Vector3Int(i, j, k);
                        var isGround = terrian.IsGround(c);

                        if (!isGround)
                        {
                            continue;
                        }

                        var dirs = new[]
                        {
                            Vector3Int.left,
                            Vector3Int.right,
                            new Vector3Int(0, 0, 1),
                            new Vector3Int(0, 0, -1),
                            Vector3Int.up
                        };

                        foreach (var dir in dirs)
                        {
                            var next = c + dir;
                            if (terrian.IsGround(next))
                            {
                                continue;
                            }

                            var axis = dir.x != 0
                                ? Axis.X
                                : dir.y != 0
                                    ? Axis.Y
                                    : Axis.Z;
                            var front = axis == Axis.X ? dir.x > 0 : axis == Axis.Y ? dir.y > 0 : dir.z > 0;
                            meshBuilder.AddQuad(axis, front, c, 0);
                        }
                    }
                }
            }

            var mesh = meshBuilder.Build();

            meshFilter.mesh = mesh;

            dirty = false;
        }
    }
}