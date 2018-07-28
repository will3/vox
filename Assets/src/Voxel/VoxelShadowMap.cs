using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class VoxelShadowMap
    {
        public enum ShadowMapState {
            Pending,
            Ready,
            Drawn
        }

        int size;
        TerrianConfig config;
        public VoxelShadowMap(int size, TerrianConfig config) {
            this.size = size;
            this.config = config;
        }

        Dictionary<Vector2Int, ShadowMapState> states = new Dictionary<Vector2Int, ShadowMapState>();
        Dictionary<Vector2Int, ComputeBuffer> buffers = new Dictionary<Vector2Int, ComputeBuffer>();
        
        static int lightY = 100;
        static int minY = -100;

        private Vector2Int GetOrigin(Vector2Int coord) {
            return new Vector2Int(
                Mathf.FloorToInt(coord.x / (float)size),
                Mathf.FloorToInt(coord.y / (float)size)
            ) * size;
        }

        public void Update() {
            var keys = new Vector2Int[states.Keys.Count];
            states.Keys.CopyTo(keys, 0);

            foreach(var origin in keys) {
                var state = states[origin];
                if (state == ShadowMapState.Pending)
                {
                    Update(origin);
                    SetState(origin, ShadowMapState.Ready);
                }
            }
        }

        void SetState(Vector2Int origin, ShadowMapState state) {
            states[origin] = state;
        }

        ShadowMapState GetState(Vector2Int origin) {
            ShadowMapState state;
            states.TryGetValue(origin, out state);
            return state;
        }

        public void ChunkDrawn(Vector3Int origin) {
            var num = config.maxChunksY + 1;
            var from = new Vector2Int(origin.x, origin.z);

            for (var i = 0; i < num; i++) {
                for (var j = 0; j < num; j++) {
                    var uv = new Vector2Int(from.x - i * size, from.y - j * size);
                    SetState(uv, ShadowMapState.Pending);
                }
            }
        }

        void Update(Vector2Int origin) {
            PerformanceLogger.Start("Shadowmap");
            // Clear
            var texture = new int[size * size];
            for (var i = 0; i < size; i ++) {
                for (var j = 0; j < size; j++) {
                    int index = i * size + j;
                    var v = CalcShadow(new Vector3Int(i + origin.x, 0, j + origin.y));
                    texture[index] = v;
                }
            }

            if (!buffers.ContainsKey(origin)) {
                buffers[origin] = new ComputeBuffer(size * size, sizeof(int));
            }

            buffers[origin].SetData(texture);
            PerformanceLogger.End();
        }

        int CalcShadow(Vector3Int coord)
        {
            var start = liftVector(coord, lightY);
            var dir = new Vector3(-1, -1, -1).normalized;
            var ray = new Ray(start, dir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return GetCoord(hit, dir).y;
            }
            else
            {
                return minY;
            }
        }

        Vector3Int liftVector(Vector3Int coord, int height)
        {
            var diff = height - coord.y;
            return new Vector3Int(coord.x + diff, coord.y + diff, coord.z + diff);
        }

        Vector3Int GetCoord(RaycastHit hit, Vector3 dir)
        {
            var point = hit.point + dir * 0.1f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }

        public void Dispose() {
            foreach(var buffer in buffers.Values) {
                buffer.Dispose();
            }
            if (defaultBuffer != null) {
                defaultBuffer.Dispose();    
            }
        }

        ComputeBuffer defaultBuffer;
        ComputeBuffer GetDefaultBuffer() {
            if (defaultBuffer == null) {
                defaultBuffer = new ComputeBuffer(size * size, sizeof(int));
            }
            return defaultBuffer;
        }

        public ComputeBuffer GetBuffer(Vector3Int origin, Vector2Int offset) {
            var o = new Vector2Int(origin.x, origin.z) + ((offset - new Vector2Int(2, 2)) * size);
            if (buffers.ContainsKey(o)) {
                return buffers[o];
            }

            return GetDefaultBuffer();
        }

        public int[] ReadShadowBuffer(ComputeBuffer buffer) {
            var data = new int[size * size];
            buffer.GetData(data);
            return data;
        }
    }
}