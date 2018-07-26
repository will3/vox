using UnityEngine;

namespace FarmVox
{
    public class VisionMap
    {
        Vector2Int origin;

        public Vector2Int Origin
        {
            get
            {
                return origin;
            }
        }

        int resolution;

        public int Resolution
        {
            get
            {
                return resolution;
            }
        }

        int size;

        public int Size
        {
            get
            {
                return size;
            }
        }

        float[] data;
        int dataSize;

        public VisionMap(int size, Vector2Int origin, int resolution = 4)
        {
            this.size = size;
            this.origin = origin;
            this.resolution = resolution;
            dataSize = size / resolution;
            data = new float[dataSize * dataSize];
        }

        void SetLocal(Vector2Int coord, float v)
        {
            var index = coord.x * dataSize + coord.y;
            data[index] = v;
        }

        float GetLocal(Vector2Int coord)
        {
            var index = coord.x * dataSize + coord.y;
            return data[index];
        }

        Vector2Int GetCoord(Vector2 position)
        {
            position -= origin;
            return new Vector2Int(
                Mathf.FloorToInt(position.x / (float)resolution),
                Mathf.FloorToInt(position.y / (float)resolution)
            );
        }

        public void Set(float x, float z, float amount) {
            Set(new Vector2(x, z), amount);
        }

        public void Set(Vector2 position, float amount)
        {
            var coord = GetCoord(position);
            SetLocal(coord, amount);
        }

        public float Get(Vector2 position)
        {
            var coord = GetCoord(position);
            return GetLocal(coord);
        }

        public void Clear()
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = 0.0f;
            }
        }

        ComputeBuffer buffer;
        public ComputeBuffer GetComputeBuffer()
        {
            if (buffer == null)
            {
                buffer = new ComputeBuffer(dataSize * dataSize, sizeof(float));
            }

            buffer.SetData(data);

            return buffer;
        }

        public void Dispose()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
        }
    }
}