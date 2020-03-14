using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    [RequireComponent(typeof(ChunksMesher))]
    public class VoxelShadowMap : MonoBehaviour
    {
        private enum ShadowMapState
        {
            Pending,
            Ready
        }

        public int size = 32;
        public int lightY = 100;

        public int maxChunksY = 4;
        public ChunksMesher chunksMesher;

        public int DataSize => size + 1;
        private const int MinY = -100;
        private static ComputeBuffer _defaultBuffer;

        private readonly Dictionary<Vector2Int, ShadowMapState> _states = new Dictionary<Vector2Int, ShadowMapState>();
        private readonly Dictionary<Vector2Int, ComputeBuffer> _buffers = new Dictionary<Vector2Int, ComputeBuffer>();
        private Vector3Int _lightDir = new Vector3Int(-1, -1, -1);
        private LightController _lightController;

        private void Start()
        {
            _lightController = FindObjectOfType<LightController>();
            if (_lightController == null)
            {
                Logger.LogComponentNotFound(typeof(LightController));
            }

            _lightDir = _lightController.lightDir;
        }

        private void Update()
        {
            var keys = new Vector2Int[_states.Keys.Count];
            _states.Keys.CopyTo(keys, 0);

            foreach (var key in keys)
            {
                var state = GetState(key);
                if (state != ShadowMapState.Pending) continue;
                UpdateChunk(key);
                var affectedOrigins = CalcChunksToUpdate(key);

                foreach (var origin in affectedOrigins)
                {
                    foreach (var chunks in chunksMesher.chunksToDraw)
                    {
                        var chunk = chunks.GetChunk(origin);
                        if (chunk == null)
                        {
                            continue;
                        }

                        var offsets = new[]
                        {
                            new Vector2Int(0, 0),
                            new Vector2Int(0, 1),
                            new Vector2Int(1, 0),
                            new Vector2Int(1, 1)
                        };

                        var y = origin.y;
                        var shadowOrigin = new Vector2Int(origin.x + y * _lightDir.x, origin.z + y * _lightDir.z);
                        var buffers = offsets.Select(offset =>
                        {
                            var o = new Vector2Int(
                                shadowOrigin.x + offset.x * size * _lightDir.x,
                                shadowOrigin.y + offset.y * size * _lightDir.z);
                            return _buffers.TryGetValue(o, out var buffer) ? buffer : GetDefaultBuffer();
                        }).ToArray();
                        
                        chunk.SetLightDir(_lightDir);
                        chunk.UpdateShadowBuffers(buffers);
                    }
                }

                SetState(key, ShadowMapState.Ready);
            }
        }

        private void OnDestroy()
        {
            foreach (var buffer in _buffers.Values)
            {
                buffer.Dispose();
            }

            _defaultBuffer?.Dispose();
        }

        public ComputeBuffer GetDefaultBuffer()
        {
            return _defaultBuffer ?? (_defaultBuffer = new ComputeBuffer(DataSize * DataSize, sizeof(int)));
        }

        public void ChunkDrawn(Vector3Int origin)
        {
            var origins = CalcChunksToRedraw(origin);

            foreach (var o in origins)
            {
                SetState(o, ShadowMapState.Pending);
            }
        }

        private void SetState(Vector2Int origin, ShadowMapState state)
        {
            _states[origin] = state;
        }

        private ShadowMapState GetState(Vector2Int origin)
        {
            return _states.TryGetValue(origin, out var state) ? state : ShadowMapState.Pending;
        }

        private IEnumerable<Vector3Int> CalcChunksToUpdate(Vector2Int key)
        {
            for (var j = 0; j < maxChunksY; j++)
            {
                for (var i = 0; i < 2; i++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var result = new Vector3Int(i - j * _lightDir.x, j, k - j * _lightDir.z) * size;
                        result.x += key.x;
                        result.z += key.y;
                        yield return result;
                    }
                }
            }
        }

        private IEnumerable<Vector2Int> CalcChunksToRedraw(Vector3Int origin)
        {
            var from = new Vector2Int(origin.x - origin.y, origin.z - origin.y);
            const int num = 2;
            var list = new List<Vector2Int>();

            for (var i = 0; i < num; i++)
            {
                for (var j = 0; j < num; j++)
                {
                    var uv = new Vector2Int(from.x + i * size * _lightDir.x, from.y + j * size * _lightDir.z);
                    list.Add(uv);
                }
            }

            return list;
        }

        private void UpdateChunk(Vector2Int origin)
        {
            PerformanceLogger.Push("Shadow Map");
            // Clear
            var texture = new int[DataSize * DataSize];
            for (var i = 0; i < DataSize; i++)
            {
                for (var j = 0; j < DataSize; j++)
                {
                    var index = i * DataSize + j;
                    var v = CalcShadow(new Vector3Int(i + origin.x, 0, j + origin.y));
                    texture[index] = v;
                }
            }

            if (!_buffers.ContainsKey(origin))
            {
                _buffers[origin] = new ComputeBuffer(DataSize * DataSize, sizeof(int));
            }

            _buffers[origin].SetData(texture);
            PerformanceLogger.Pop();
        }

        private int CalcShadow(Vector3Int coord)
        {
            var diff = coord.y - lightY;
            var start = coord + _lightDir * diff;
            var ray = new Ray(start, _lightDir);

            return Physics.Raycast(ray, out var hit) ? GetCoord(hit, _lightDir).y : MinY;
        }

        private static Vector3Int GetCoord(RaycastHit hit, Vector3 dir)
        {
            var point = hit.point + dir * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                Mathf.FloorToInt(point.y),
                Mathf.FloorToInt(point.z));
        }
    }
}