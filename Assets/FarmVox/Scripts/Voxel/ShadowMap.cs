using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class ShadowMap : MonoBehaviour
    {
        public int size = 32;
        public int lightY = 100;
        public int maxChunksY = 4;
        public LightDir lightDir;

        public Vector3Int LightDirVector => lightDir.GetDirVector();

        private int DataSize => size + 1;
        private const int MinY = -100;
        private static ComputeBuffer _defaultBuffer;

        private readonly Dictionary<Vector2Int, ComputeBuffer> _buffers = new Dictionary<Vector2Int, ComputeBuffer>();
        private readonly HashSet<Vector2Int> _dirtyShadowMapChunks = new HashSet<Vector2Int>();
        private readonly HashSet<Vector3Int> _dirtyChunks = new HashSet<Vector3Int>();
        private readonly HashSet<Vector3Int> _allChunks = new HashSet<Vector3Int>();

        public void UpdateAllChunks()
        {
            _dirtyChunks.UnionWith(_allChunks);
        }

        public void UpdateBuffers()
        {
            foreach (var origin in _dirtyShadowMapChunks)
            {
                CalculateShadowChunk(origin);
                var affectedChunks = CalcChunksToUpdate(origin);
                foreach (var chunkOrigin in affectedChunks)
                {
                    _dirtyChunks.Add(chunkOrigin);
                }
            }

            var dirtyShadowMapChunksCount = _dirtyShadowMapChunks.Count;

            _dirtyShadowMapChunks.Clear();

            foreach (var origin in _dirtyChunks)
            {
                _allChunks.Add(origin);
                var offsets = new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1)
                };

                var y = origin.y;
                var shadowOrigin = new Vector2Int(origin.x + y * LightDirVector.x, origin.z + y * LightDirVector.z);

                var buffers = offsets.Select(offset =>
                {
                    var o = new Vector2Int(
                        shadowOrigin.x + offset.x * size * LightDirVector.x,
                        shadowOrigin.y + offset.y * size * LightDirVector.z);
                    return _buffers.TryGetValue(o, out var buffer) ? buffer : GetDefaultBuffer();
                }).ToArray();
                ShadowEvents.Instance.PublishShadowMapUpdated(origin, DataSize, buffers);
            }

            if (dirtyShadowMapChunksCount > 0 || _dirtyChunks.Count > 0)
            {
                Debug.Log($"Redrawn {dirtyShadowMapChunksCount} chunks, Updated {_dirtyChunks.Count} chunks");    
            }

            _dirtyChunks.Clear();
        }

        private void OnDestroy()
        {
            foreach (var buffer in _buffers.Values)
            {
                buffer.Dispose();
            }

            _defaultBuffer?.Dispose();
        }

        public static ComputeBuffer GetDefaultBuffer()
        {
            return _defaultBuffer ?? (_defaultBuffer = new ComputeBuffer(1, sizeof(int)));
        }

        private IEnumerable<Vector3Int> CalcChunksToUpdate(Vector2Int key)
        {
            for (var j = 0; j < maxChunksY; j++)
            {
                for (var i = 0; i < 2; i++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var result = new Vector3Int(i - j * LightDirVector.x, j, k - j * LightDirVector.z) * size;
                        result.x += key.x;
                        result.z += key.y;
                        yield return result;
                    }
                }
            }
        }

        public IEnumerable<Vector2Int> CalcChunksToRedraw(Vector3Int origin)
        {
            var from = new Vector2Int(origin.x - origin.y, origin.z - origin.y);
            const int num = 2;
            var list = new List<Vector2Int>();

            for (var i = 0; i < num; i++)
            {
                for (var j = 0; j < num; j++)
                {
                    var uv = new Vector2Int(from.x + i * size * LightDirVector.x, from.y + j * size * LightDirVector.z);
                    list.Add(uv);
                }
            }

            return list;
        }

        private void CalculateShadowChunk(Vector2Int origin)
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
            var start = coord + LightDirVector * diff;
            var ray = new Ray(start, LightDirVector);

            return Physics.Raycast(ray, out var hit) ? GetCoord(hit, LightDirVector).y : MinY;
        }

        private static Vector3Int GetCoord(RaycastHit hit, Vector3 dir)
        {
            var point = hit.point + dir * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                Mathf.FloorToInt(point.y),
                Mathf.FloorToInt(point.z));
        }

        public void SetDirty(Vector2Int origin)
        {
            _dirtyShadowMapChunks.Add(origin);
        }
    }
}