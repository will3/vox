using UnityEngine;
using System.Collections;

namespace FarmVox
{
    public class Array3<T>
    {
        private int width;

        public int Width
        {
            get
            {
                return width;
            }
        }

        private int height;

        public int Height
        {
            get
            {
                return height;
            }
        }

        private int depth;

        public int Depth
        {
            get
            {
                return depth;
            }
        }

        private T[] data;

        public Array3(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            data = new T[width * height * depth];
        }

        public T Get(int i, int j, int k)
        {
            var index = i * depth * height + j * depth + k;
            return data[index];
        }

        public void Set(int i, int j, int k, T v)
        {
            var index = i * depth * height + j * depth + k;
            data[index] = v;
        }

        public void Clear() {
            data = new T[width * height * depth];
        }
    }
}