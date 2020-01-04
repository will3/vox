using System;
using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox
{
    public class CubeMeshBuilder
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<int> _indices = new List<int>();
        private readonly List<Vector2> _uvs = new List<Vector2>();

        public Mesh Build()
        {
            var mesh = new Mesh();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_indices.ToArray(), 0);
            return mesh;
        }

        public CubeMeshBuilder AddQuad(
            Axis d,
            bool front,
            Vector3 offset,
            float dirOffset = 0.1f,
            float sizeOffset = 0.0f)
        {
            var diffI = front ? 1.0f : 0.0f;

            var dirOffsetVector = GetDir(d, front) * dirOffset;

            var min = 0 - sizeOffset;
            var max = 1.0f + sizeOffset;
            var v0 = GetVector(diffI, min, min, d) + offset + dirOffsetVector;
            var v1 = GetVector(diffI, max, min, d) + offset + dirOffsetVector;
            var v2 = GetVector(diffI, max, max, d) + offset + dirOffsetVector;
            var v3 = GetVector(diffI, min, max, d) + offset + dirOffsetVector;

            var index = _vertices.Count;

            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            if (front)
            {
                _indices.Add(index + 0);
                _indices.Add(index + 1);
                _indices.Add(index + 2);
                _indices.Add(index + 2);
                _indices.Add(index + 3);
                _indices.Add(index + 0);
            }
            else
            {
                _indices.Add(index + 2);
                _indices.Add(index + 1);
                _indices.Add(index + 0);
                _indices.Add(index + 0);
                _indices.Add(index + 3);
                _indices.Add(index + 2);
            }

            return this;
        }

        private static Vector3 GetVector(float i, float j, float k, Axis d)
        {
            switch (d)
            {
                case Axis.X:
                    return new Vector3(i, j, k);
                case Axis.Y:
                    return new Vector3(k, i, j);
                case Axis.Z:
                    return new Vector3(j, k, i);
                default:
                    throw new ArgumentOutOfRangeException("d", d, null);
            }
        }

        private static Vector3 GetDir(Axis d, bool front)
        {
            var multiplier = front ? 1.0f : -1.0f;
            switch (d)
            {
                case Axis.X:
                    return new Vector3(1, 0, 0) * multiplier;
                case Axis.Y:
                    return new Vector3(0, 1, 0) * multiplier;
                case Axis.Z:
                    return new Vector3(0, 0, 1) * multiplier;
                default:
                    throw new ArgumentOutOfRangeException("d", d, null);
            }
        }
    }
}