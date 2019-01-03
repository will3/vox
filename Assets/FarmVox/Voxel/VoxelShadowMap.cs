using System.Collections.Generic;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class VoxelShadowMap
    {
        private enum ShadowMapState {
            Pending,
            Ready,
            Drawn
        }

        public int DataSize { get; private set; }

        public int Size { get; private set; }

        readonly TerrianConfig _config;
        
        public VoxelShadowMap(int size, TerrianConfig config)
        {
            Size = size;
            DataSize = size + 1;
            _config = config;
        }

        readonly Dictionary<Vector2Int, ShadowMapState> _states = new Dictionary<Vector2Int, ShadowMapState>();
        readonly Dictionary<Vector2Int, ComputeBuffer> _buffers = new Dictionary<Vector2Int, ComputeBuffer>();

        private const int LightY = 100;
        private const int MinY = -100;

        private Vector2Int GetOrigin(Vector2Int coord)
        {
            return new Vector2Int(
                Mathf.FloorToInt(coord.x / (float)Size),
                Mathf.FloorToInt(coord.y / (float)Size)
            ) * Size;
        }

        public void Update()
        {
            var keys = new Vector2Int[_states.Keys.Count];
            _states.Keys.CopyTo(keys, 0);

            foreach (var origin in keys)
            {
                var state = _states[origin];
                if (state != ShadowMapState.Pending) continue;
                Update(origin);
                SetState(origin, ShadowMapState.Ready);
            }
        }

        void SetState(Vector2Int origin, ShadowMapState state) {
            _states[origin] = state;
        }

        ShadowMapState GetState(Vector2Int origin) {
            ShadowMapState state;
            _states.TryGetValue(origin, out state);
            return state;
        }

        public void ChunkDrawn(Vector3Int origin)
        {
            const int num = 2;
            var from = new Vector2Int(origin.x - origin.y, origin.z - origin.y);

            for (var i = 0; i < num; i++)
            {
                for (var j = 0; j < num; j++)
                {
                    var uv = new Vector2Int(from.x - i * Size, from.y - j * Size);
                    SetState(uv, ShadowMapState.Pending);
                }
            }
        }

        void Update(Vector2Int origin)
        {
            PerformanceLogger.Push("Shadowmap");
            // Clear
            var texture = new int[DataSize * DataSize];
            for (var i = 0; i < DataSize; i++)
            {
                for (var j = 0; j < DataSize; j++)
                {
                    int index = i * DataSize + j;
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

        int CalcShadow(Vector3Int coord)
        {
            var start = LiftVector(coord, LightY);
            
            var dir = new Vector3(-1, -1, -1).normalized;
            var ray = new Ray(start, dir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return GetCoord(hit, dir).y;
            }

            return MinY;
        }

        static Vector3Int LiftVector(Vector3Int coord, int height)
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

        public void Dispose()
        {
            foreach (var buffer in _buffers.Values)
            {
                buffer.Dispose();
            }
            
            if (_defaultBuffer != null)
            {
                _defaultBuffer.Dispose();
            }
        }

        private ComputeBuffer _defaultBuffer;

        private ComputeBuffer GetDefaultBuffer()
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
            var o = new Vector2Int(origin.x - y, origin.z - y) - offset * Size;

            return _buffers.ContainsKey(o) ? _buffers[o] : GetDefaultBuffer();
        }

        public int[] ReadShadowBuffer(ComputeBuffer buffer)
        {
            var data = new int[DataSize * DataSize];
            buffer.GetData(data);
            return data;
        }

        public void Rebuild() {
            var keys = new Vector2Int[_states.Keys.Count];
            _states.Keys.CopyTo(keys, 0);

            foreach (var key in keys) {
                _states[key] = ShadowMapState.Pending;
            }

            Update();
        }

        public void UpdateMaterial(Material material, Vector3Int origin)
        {
            material.SetBuffer("_ShadowMap00", GetBuffer(origin, new Vector2Int(0, 0)));
            material.SetBuffer("_ShadowMap01", GetBuffer(origin, new Vector2Int(0, 1)));
            material.SetBuffer("_ShadowMap10", GetBuffer(origin, new Vector2Int(1, 0)));
            material.SetBuffer("_ShadowMap11", GetBuffer(origin, new Vector2Int(1, 1)));
            material.SetInt("_ShadowMapSize", DataSize);
            material.SetFloat("_ShadowStrength", _config.ShadowStrength);
        }
    }
}