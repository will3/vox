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
        public VoxelShadowMap(int size) {
            this.size = size;
        }

        Dictionary<Vector2Int, int[]> data = new Dictionary<Vector2Int, int[]>();
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
            UpdateOne();
        }

        public void UpdateOne() {
            foreach (var kv in states)
            {
                var origin = kv.Key;
                var state = kv.Value;
                if (state == ShadowMapState.Pending)
                {
                    Update(origin);
                    break;
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
            int num = origin.y / size + 1;
            var from = new Vector2Int(origin.x, origin.z);

            for (var i = 0; i < num; i++) {
                for (var j = 0; j < num; j++) {
                    var uv = new Vector2Int(from.x - i * size, from.y - j * size);
                    SetState(uv, ShadowMapState.Pending);
                }
            }
        }

        void Update(Vector2Int origin) {
            // Clear
            data[origin] = new int[size * size];
            var texture = data[origin];
            for (var i = 0; i < size; i ++) {
                for (var j = 0; j < size; j++) {
                    int index = i * size + j;
                    var v = CalcShadow(new Vector3Int(i, 0, j));
                    texture[index] = v;
                }
            }

            if (!buffers.ContainsKey(origin)) {
                buffers[origin] = new ComputeBuffer(size * size, sizeof(float));
            }

            buffers[origin].SetData(texture);
        }

        int CalcShadow(Vector3Int coord)
        {
            var start = liftVector(coord, lightY) + new Vector3(0.5f, 0, 0.5f);
            var ray = new Ray(start, new Vector3(-1, -1, -1));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return GetCoord(hit).y;
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

        Vector3Int GetCoord(RaycastHit hit)
        {
            var point = hit.point - hit.normal * 0.5f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }

        public void Dispose() {
            foreach(var buffer in buffers.Values) {
                buffer.Dispose();
            }
        }
    }
}