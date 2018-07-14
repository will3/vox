using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelMesher
{
    public static IList<Vector3Int> GetSurfaceVoxels(Chunk chunk)
    {
        var size = chunk.Size;
        var list = new List<Vector3Int>();
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                for (var k = 0; k < size; k++)
                {
                    var v = chunk.GetGlobal(i, j, k);
                    var left = chunk.GetGlobal(i - 1, j, k);
                    var bot = chunk.GetGlobal(i, j - 1, k);
                    var back = chunk.GetGlobal(i, j, k - 1);
                    var order = v > 0;

                    if (v > 0 != left > 0)
                    {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i - 1, j, k);
                        list.Add(coord);
                    }

                    if (v > 0 != bot > 0)
                    {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j - 1, k);
                        list.Add(coord);
                    }

                    if (v > 0 != back > 0)
                    {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j, k - 1);
                        list.Add(coord);
                    }
                }
            }
        }

        return list;
    }

    public static void Mesh(Chunk chunk, Mesh mesh) {
        var size = chunk.Size;
        var verts = new List<Vector3>();
        var indices = new List<int>();
        var colors = new List<Color>();

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j ++) {
                for (var k = 0; k < size; k++) {
                    var v = chunk.GetGlobal(i, j, k);
                    var left = chunk.GetGlobal(i - 1, j, k);
                    var bot = chunk.GetGlobal(i, j - 1, k);
                    var back = chunk.GetGlobal(i, j, k - 1);
                    var order = v > 0;
                    var c = chunk.GetColorGlobal(i, j, k);
                    var l = chunk.GetLightingGlobal(i, j, k);
                    if (v > 0 != left > 0) {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i - 1, j, k);
                        var color = chunk.GetColorGlobal(coord.x, coord.y, coord.z) * chunk.GetLightingGlobal(coord.x, coord.y, coord.z);

                        AddQuad(
                            new Vector3(i, j, k),
                            new Vector3(i, j + 1, k),
                            new Vector3(i, j + 1, k + 1),
                            new Vector3(i, j, k + 1),
                            color,
                            verts,
                            indices,
                            colors,
                            !order
                        );
                    }

                    if (v > 0 != bot > 0) {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j - 1, k);
                        var color = chunk.GetColorGlobal(coord.x, coord.y, coord.z) * chunk.GetLightingGlobal(coord.x, coord.y, coord.z);

                        AddQuad(
                            new Vector3(i, j, k),
                            new Vector3(i + 1, j, k),
                            new Vector3(i + 1, j, k + 1),
                            new Vector3(i, j, k + 1),
                            color,
                            verts,
                            indices,
                            colors,
                            order
                        );
                    }

                    if (v > 0 != back > 0)
                    {
                        var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j, k - 1);
                        var color = chunk.GetColorGlobal(coord.x, coord.y, coord.z) * chunk.GetLightingGlobal(coord.x, coord.y, coord.z);

                        AddQuad(
                            new Vector3(i, j, k),
                            new Vector3(i + 1, j, k),
                            new Vector3(i + 1, j + 1, k),
                            new Vector3(i, j + 1, k),
                            color,
                            verts,
                            indices,
                            colors,
                            !order
                        );
                    }
                }
            }
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.SetColors(colors);
    }

    private static void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color, IList<Vector3> verts, IList<int> indices, IList<Color> colors, bool order) {
        var index = verts.Count;
        verts.Add(a);
        verts.Add(b);
        verts.Add(c);
        verts.Add(d);

        if (order) {
            indices.Add(index);
            indices.Add(index + 1);
            indices.Add(index + 2);
            indices.Add(index);
            indices.Add(index + 2);
            indices.Add(index + 3);    
        } else {
            indices.Add(index + 2);
            indices.Add(index + 1);
            indices.Add(index);
            indices.Add(index + 3);
            indices.Add(index + 2);
            indices.Add(index);       
        }

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}
