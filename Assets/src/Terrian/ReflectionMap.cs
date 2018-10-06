using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class ReflectionMap
    {
        public class ReflectionMapChunk : IDisposable
        {
            ComputeBuffer buffer;

            public ComputeBuffer Buffer
            {
                get
                {
                    return buffer;
                }
            }

            Color[] data;
            int size;

            public ReflectionMapChunk(int size) {
                this.size = size;
            }

            public void Clear() {
                data = null;
                if (buffer != null) {
                    buffer.Dispose();
                    buffer = null;
                }
            }

            public void Dispose()
            {
                if (buffer != null) {
                    buffer.Dispose();
                }
            }

            public void Set(Vector2Int xz, Color color) {
                var index = xz.x * size + xz.y;
                if (data == null) {
                    data = new Color[size * size];
                }
                data[index] = color;
            }

            public void UpdateBuffer() {
                if (data != null) {
                    if (buffer != null) {
                        buffer = new ComputeBuffer(size * size, sizeof(float) * 4);
                    }

                    buffer.SetData(data);
                }
            }

            public ComputeBuffer GetBuffer() {
                return buffer;
            }
        }

        Dictionary<Vector3Int, ReflectionMapChunk> map = 
            new Dictionary<Vector3Int, ReflectionMapChunk>();
        Chunks chunks;
        int size;

        public ReflectionMap(Chunks chunks, int size) {
            this.chunks = chunks;
            this.size = size;
        }

        public void LoadChunk(Chunk waterChunk) {
            if (!map.ContainsKey(waterChunk.Origin)) {
                map[waterChunk.Origin] = new ReflectionMapChunk(size);
            }

            var reflectionMapChunk = map[waterChunk.Origin];
            reflectionMapChunk.Clear();

            var coords = waterChunk.surfaceCoordsUp;

            foreach(var coord in coords) {
                var pos = Camera.main.transform.position;
                var target = new Vector3(coord.x + 0.5f, coord.y + 1.0f, coord.z + 0.5f);
                var dir = target - pos;
                dir.y *= -1.0f;

                var ray = new Ray(target, dir);

                var result = VoxelRaycast.TraceRay(ray);
                if (result != null) {
                    var color = result.GetColor(chunks);
                    var xz = Vectors.GetXZ(coord);
                    reflectionMapChunk.Set(xz, color);
                }

                reflectionMapChunk.UpdateBuffer();
            }
        }
    }
}
