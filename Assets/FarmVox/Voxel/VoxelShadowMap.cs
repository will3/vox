using System.Collections.Generic;
using FarmVox.Scripts;
using UnityEngine;

namespace FarmVox.Voxel
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
        public int minY = -100;
        public int maxChunksY = 4;
        public ChunksMesher chunksMesher;

        public int DataSize => size + 1;
        private Dictionary<Vector2Int, ShadowMapState> _states;
        private Dictionary<Vector2Int, ComputeBuffer> _buffers;

        private void Start()
        {
            _states = new Dictionary<Vector2Int, ShadowMapState>();
            _buffers = new Dictionary<Vector2Int, ComputeBuffer>();
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
                var affectedOrigins = CalcAffectedOrigins(key);

                foreach (var origin in affectedOrigins)
                {
                    foreach (var chunks in chunksMesher.chunksToDraw)
                    {
                        var chunk = chunks.GetChunk(origin);
                        if (chunk == null)
                        {
                            continue;
                        }

                        chunk.UpdateShadowBuffers(
                            GetBuffer(origin, new Vector2Int(0, 0)),
                            GetBuffer(origin, new Vector2Int(0, 1)),
                            GetBuffer(origin, new Vector2Int(1, 0)),
                            GetBuffer(origin, new Vector2Int(1, 1)));
                    }    
                }

                SetState(key, ShadowMapState.Ready);
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

        public void ChunkDrawn(Vector3Int origin)
        {
            var origins = CalcAffectedKeys(origin);

            foreach (var o in origins)
            {
                SetState(o, ShadowMapState.Pending);
            }
        }

        private IEnumerable<Vector3Int> CalcAffectedOrigins(Vector2Int key)
        {
            for (var j = 0; j < maxChunksY; j++)
            {
                for (var i = 0; i < 2; i++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var result = new Vector3Int(i + j, j, k + j) * size;
                        result.x += key.x;
                        result.z += key.y;
                        yield return result;
                    }
                }
            }           
        }

        private IEnumerable<Vector2Int> CalcAffectedKeys(Vector3Int origin)
        {
            var from = new Vector2Int(origin.x - origin.y, origin.z - origin.y);
            const int num = 2;
            var list = new List<Vector2Int>();

            for (var i = 0; i < num; i++)
            {
                for (var j = 0; j < num; j++)
                {
                    var uv = new Vector2Int(from.x - i * size, from.y - j * size);
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
            var start = LiftVector(coord, lightY);

            var dir = new Vector3(-1, -1, -1).normalized;
            var ray = new Ray(start, dir);

            return Physics.Raycast(ray, out var hit) ? GetCoord(hit, dir).y : minY;
        }

        private static Vector3Int LiftVector(Vector3Int coord, int height)
        {
            var diff = height - coord.y;
            return new Vector3Int(coord.x + diff, coord.y + diff, coord.z + diff);
        }

        private static Vector3Int GetCoord(RaycastHit hit, Vector3 dir)
        {
            var point = hit.point + dir * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                Mathf.FloorToInt(point.y),
                Mathf.FloorToInt(point.z));
        }

        public void OnDestroy()
        {
            foreach (var buffer in _buffers.Values)
            {
                buffer.Dispose();
            }

            _defaultBuffer?.Dispose();
        }

        private static ComputeBuffer _defaultBuffer;

        public ComputeBuffer GetDefaultBuffer()
        {
            return _defaultBuffer ?? (_defaultBuffer = new ComputeBuffer(DataSize * DataSize, sizeof(int)));
        }

        public ComputeBuffer GetBuffer(Vector3Int origin, Vector2Int offset)
        {
            if (offset.x >= 2 || offset.y >= 2 || offset.x < 0 || offset.y < 0)
            {
                throw new System.ArgumentException("offset must be within 0 - 2");
            }

            var y = origin.y;
            var o = new Vector2Int(origin.x - y, origin.z - y) - offset * size;

            return _buffers.ContainsKey(o) ? _buffers[o] : GetDefaultBuffer();
        }
    }
}