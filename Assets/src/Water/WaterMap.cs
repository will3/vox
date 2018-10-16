using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class WaterMap : IDisposable
    {
        public readonly Dictionary<Vector3Int, WaterChunk> Map = new Dictionary<Vector3Int, WaterChunk>();
        readonly int size;
        readonly TerrianConfig config;

        public WaterMap(int size, TerrianConfig config) {
            this.size = size;
            this.config = config;
        }

        public void AddWater(Vector3Int coord) {
            if (coord.y == config.ActualWaterLevel - 1) {
                var origin = Origins.GetOrigin(coord.x, coord.y, coord.z, size);
                var chunk = GetWaterChunk(origin);
                chunk.AddSurfaceCoord(coord);
            }
        }

        public void Dispose()
        {
            foreach(var chunk in Map.Values) {
                chunk.Dispose();
            }
        }

        public WaterChunk GetWaterChunk(Vector3Int origin) {
            if (!Map.ContainsKey(origin))
            {
                Map[origin] = new WaterChunk(size, origin);
            }
            return Map[origin];
        }
    }

    public class WaterChunk : IDisposable {
        readonly HashSet<Vector3Int> surfaceCoords = new HashSet<Vector3Int>();
        readonly int size;
        public ComputeBuffer ReflectionBuffer { get; private set; }
        readonly Vector3Int origin;

        public WaterChunk(int size, Vector3Int origin)
        {
            this.size = size;
            this.origin = origin;
        }

        public void AddSurfaceCoord(Vector3Int coord) {
            surfaceCoords.Add(coord);
            if (ReflectionBuffer == null) {
                ReflectionBuffer = new ComputeBuffer(size * size, sizeof(float) * 4);
            }
        }

        public void UpdateReflection() {
            if (ReflectionBuffer == null) {
                return;
            }

            Color[] data = new Color[size * size];

            foreach (var surfaceCoord in surfaceCoords) {
                var pos = surfaceCoord + new Vector3(0.5f, 1.0f, 0.5f);
                var dir = (pos - Camera.main.transform.position).normalized;
                dir.y *= -1.0f;
                var ray = new Ray(pos, dir);
                var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

                if (result != null) {
                    var localCoord = surfaceCoord - origin;
                    var index = localCoord.x * size + localCoord.z;
                    data[index] = result.GetColor(Finder.FindTerrian().DefaultLayer);
                }
            }

            ReflectionBuffer.SetData(data);
        }

        public void Dispose()
        {
            if (ReflectionBuffer != null) {
                ReflectionBuffer.Dispose();
            }
        }
    }
}