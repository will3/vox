using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class VoxelMesher
    {
        private static Vector3Int getVector(int i, int j, int k, int d)
        {
            if (d == 0)
            {
                return new Vector3Int(i, j, k);
            }
            else if (d == 1)
            {
                return new Vector3Int(k, i, j);
            }
            return new Vector3Int(j, k, i);
        }

        public static void Mesh(Chunk chunk, Mesh mesh)
        {
            var size = chunk.Size;
            var verts = new List<Vector3>();
            var indices = new List<int>();
            var colors = new List<Color>();

            for (var d = 0; d < 3; d++)
            {
                var u = (d + 1) % 3;
                var v = (d + 2) % 3;

                for (var i = 0; i < size; i++)
                {

                    for (var j = 0; j < size; j++)
                    {

                        for (var k = 0; k < size; k++)
                        {

                            var coordA = getVector(i, j, k, d);
                            var coordB = getVector(i + 1, j, k, d);
                            var a = chunk.GetGlobal(coordA.x, coordA.y, coordA.z);
                            var b = chunk.GetGlobal(coordB.x, coordB.y, coordB.z);

                            if (a > 0 == b > 0)
                            {
                                continue;
                            }

                            var front = a > 0;

                            var c = front ? new Vector3Int(i, j, k) : new Vector3Int(i + 1, j, k);
                            var coord = getVector(c.x, c.y, c.z, d);

                            float lightNormal;
                            if (chunk.lightNormals.ContainsKey(coord)) {
                                lightNormal = chunk.lightNormals[coord];
                            } else {
                                lightNormal = 1.0f;
                            }

                            var shadow = chunk.GetLightingGlobal(coord.x, coord.y, coord.z);
                            var dot = lightNormalToLight(lightNormal);
                            var color = chunk.GetColor(coord.x, coord.y, coord.z) * (1 - shadow) * (1 - dot);

                            var aoI = front ? i + 1 : i;
                            // AO
                            var c00 = getVector(aoI, j - 1, k - 1, d);
                            var c01 = getVector(aoI, j, k - 1, d);
                            var c02 = getVector(aoI, j + 1, k - 1, d);
                            var c10 = getVector(aoI, j - 1, k, d);
                            var c12 = getVector(aoI, j + 1, k, d);
                            var c20 = getVector(aoI, j - 1, k + 1, d);
                            var c21 = getVector(aoI, j, k + 1, d);
                            var c22 = getVector(aoI, j + 1, k + 1, d);

                            var v00 = chunk.GetGlobal(c00);
                            var v01 = chunk.GetGlobal(c01);
                            var v02 = chunk.GetGlobal(c02);
                            var v10 = chunk.GetGlobal(c10);
                            var v12 = chunk.GetGlobal(c12);
                            var v20 = chunk.GetGlobal(c20);
                            var v21 = chunk.GetGlobal(c21);
                            var v22 = chunk.GetGlobal(c22);

                            var ao00 = getAO(v10, v01, v00);
                            var ao01 = getAO(v01, v12, v02);
                            var ao11 = getAO(v12, v21, v22);
                            var ao10 = getAO(v21, v10, v20);

                            bool flipped = ao00 + ao11 > ao01 + ao10;

                            AddQuad(
                                getVector(i + 1, j, k, d),
                                getVector(i + 1, j + 1, k, d),
                                getVector(i + 1, j + 1, k + 1, d),
                                getVector(i + 1, j, k + 1, d),
                                color,
                                ao00,
                                ao01,
                                ao11,
                                ao10,
                                verts,
                                indices,
                                colors,
                                front,
                                flipped
                            );
                        }
                    }
                }
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(indices, 0);
            mesh.SetColors(colors);
        }

        private static float ao0 = 0.0f;
        private static float ao1 = 0.33f;
        private static float ao2 = 0.66f;
        private static float ao3 = 1.0f;

        private static float getAO(float s1, float s2, float c)
        {
            if (s1 > 0 && s2 > 0)
            {
                return ao3;
            }

            var count = 0;
            if (s1 > 0) count++;
            if (s2 > 0) count++;
            if (c > 0) count++;
            if (count == 0) return ao0;
            if (count == 1) return ao1;
            return ao2;
        }

        private static void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color, float ao00, float ao01, float ao11, float ao10, IList<Vector3> verts, IList<int> indices, IList<Color> colors, bool order, bool flipped)
        {
            var index = verts.Count;
            verts.Add(a);
            verts.Add(b);
            verts.Add(c);
            verts.Add(d);

            if (order)
            {
                if (flipped)
                {
                    indices.Add(index);
                    indices.Add(index + 1);
                    indices.Add(index + 3);
                    indices.Add(index + 1);
                    indices.Add(index + 2);
                    indices.Add(index + 3);
                }
                else
                {
                    indices.Add(index);
                    indices.Add(index + 1);
                    indices.Add(index + 2);
                    indices.Add(index);
                    indices.Add(index + 2);
                    indices.Add(index + 3);
                }
            }
            else
            {
                if (flipped)
                {
                    indices.Add(index + 3);
                    indices.Add(index + 1);
                    indices.Add(index);
                    indices.Add(index + 3);
                    indices.Add(index + 2);
                    indices.Add(index + 1);
                }
                else
                {
                    indices.Add(index + 2);
                    indices.Add(index + 1);
                    indices.Add(index);
                    indices.Add(index + 3);
                    indices.Add(index + 2);
                    indices.Add(index);
                }
            }

            colors.Add(color * aoToLight(ao00));
            colors.Add(color * aoToLight(ao01));
            colors.Add(color * aoToLight(ao11));
            colors.Add(color * aoToLight(ao10));
        }

        private static float aoToLight(float ao)
        {
            return 1.0f - ao * 0.1f;
        }

        private static float lightNormalToLight(float lightNormal) {
            return (1.0f - lightNormal) * 0.4f;
        }
    }
}