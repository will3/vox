using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class BuildGrid
    {
        int size;
        int gridSize = 6;
        public BuildGrid(int size) {
            this.size = size;
        }

        class BuildGridChunk {
            int size;
            int gridSize;
            Vector2Int origin;
            int dataSize;
            public BuildGrid buildGrid;

            public BuildGridChunk(Vector2Int origin, int gridSize, int size)
            {
                this.size = size;
                this.gridSize = gridSize;
                this.origin = origin;
                dataSize = size; // Mathf.CeilToInt((float)size / (float)gridSize);
            }

            readonly Dictionary<Vector2Int, int> heightMap = new Dictionary<Vector2Int, int>();

            ComputeBuffer buffer;

            public ComputeBuffer Buffer
            {
                get
                {
                    if (buffer == null)
                    {
                        buffer = new ComputeBuffer(dataSize * dataSize, sizeof(int));
                    }
                    return buffer;
                }
            }

            public void LoadColumn(TerrianColumn column) {
                float y = 200;
                for (var i = 0; i < column.Size; i++)
                {
                    for (var k = 0; k < column.Size; k++)
                    {
                        var x = i + origin.x;
                        var z = k + origin.y;
                        var vector = new Vector3(x, y, z);

                        var ray = new Ray(vector, Vector3.down);

                        var result = VoxelRaycast.TraceRay(ray, 1 >> UserLayers.terrian);

                        if (result != null)
                        {
                            var coord = result.GetCoord();
                            heightMap[new Vector2Int(x, z)] = coord.y;
                        }
                        else
                        {
                            heightMap[new Vector2Int(x, z)] = 999;
                        }
                    }
                }

                UpdateBuffer();
            }

            void UpdateBuffer() {
                var data = new int[dataSize * dataSize];
                for (var i = 0; i < size; i++)
                {
                    for (var j = 0; j < size; j++)
                    {
                        // TODO
                        //var index = i * dataSize + j;
                        //data[index] = GetLocalHeight(i, j);
                    }
                }
                Buffer.SetData(data);
            }

            public int GetLocalHeight(Vector2Int coord) {
                return heightMap[coord];
            }

            public int GetHeight(Vector2Int coord) {
                var chunk = buildGrid.GetChunk(coord);
                return chunk.GetLocalHeight(coord);
            }

            public void Dispose() {
                buffer.Dispose();
            }
        }

        Dictionary<Vector2Int, BuildGridChunk> map = new Dictionary<Vector2Int, BuildGridChunk>();

        public void LoadColumn(TerrianColumn column) {
            var origin = Vectors.GetXZ(column.Origin);

            if (map[origin] == null) {
                map[origin] = new BuildGridChunk(origin, gridSize, size);
                map[origin].buildGrid = this;
            }

            var chunk = map[origin];
        }

        BuildGridChunk GetChunk(Vector2Int origin) {
            if (map.ContainsKey(origin)) {
                return map[origin];
            }
            return null;
        }

        public void Dispose() {
            foreach(var chunk in map.Values) {
                chunk.Dispose();
            }
        }

        static ComputeBuffer defaultBuffer;

        public ComputeBuffer GetBuffer(Vector3Int origin) {
            var key = Vectors.GetXZ(origin);
            if (!map.ContainsKey(key)) {
                if (defaultBuffer == null) {
                    defaultBuffer = new ComputeBuffer(size * size, sizeof(int));
                }
                return defaultBuffer;
            }

            return map[key].Buffer;
        }

        //class Grid {

        //}

        //int gridSize = 6;

        //Dictionary<Vector2Int, int> heightMap = new Dictionary<Vector2Int, int>();
        //Dictionary<Vector2Int, Grid> grids = new Dictionary<Vector2Int, Grid>();

        //public void LoadColumn(TerrianColumn column) {


        //    var startI = Mathf.FloorToInt((float)origin.x / (float)gridSize);
        //    var endI = Mathf.CeilToInt((float)(origin.x + column.Size) / (float)gridSize);

        //    var startI = Mathf.FloorToInt((float)origin.x / (float)gridSize);
        //    var endI = Mathf.CeilToInt((float)(origin.x + column.Size) / (float)gridSize);

        //    for (var i = )
        //}
    }
}