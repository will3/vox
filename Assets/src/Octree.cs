using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    class Octree<T>
    {
        public Vector3Int Start { get; private set; }
        public Vector3Int Size { get; private set; }
        public BoundsInt Bounds { get; private set; }

        private const int MinSize = 4;
        private const int MaxValues = 32;

        readonly List<Octree<T>> children = new List<Octree<T>>();
        readonly Dictionary<Vector3Int, T> values = new Dictionary<Vector3Int, T>();

        public Octree(Vector3Int start, Vector3Int size)
        {
            Start = start;
            Size = size;
            Bounds = new BoundsInt {min = start, max = start + size};
        }

        public Octree(BoundsInt bounds)
        {
            Bounds = bounds;
            Start = Vectors.FloorToInt(bounds.min);
            Size = Vectors.FloorToInt(bounds.size);
        }

        public bool Contains(Vector3Int pos)
        {
            return Bounds.Contains(pos);
        }

        public bool Set(Vector3Int pos, T value)
        {
            if (!Bounds.Contains(pos))
            {
                return false;
            }

            var leaf = values.Count < MaxValues || Size.x <= MinSize;

            if (leaf)
            {
                values[pos] = value;
                return true;
            }

            Divide();

            foreach (var child in children)
            {
                if (child.Set(pos, value))
                {
                    return true;
                }
            }

            return false;
        }

        private void Divide()
        {
            if (children.Count > 0)
            {
                return;
            }

            var halfX = Size.x / 2;
            var halfY = Size.y / 2;
            var halfZ = Size.z / 2;

            var halfSize = new Vector3Int(halfX, halfY, halfZ);

            children.Add(new Octree<T>(new Vector3Int(Start.x, Start.y, Start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x + halfX, Start.y, Start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x, Start.y + halfY, Start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x + halfX, Start.y + halfY, Start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x, Start.y, Start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x + halfX, Start.y, Start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x, Start.y + halfY, Start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(Start.x + halfX, Start.y + halfY, Start.z + halfZ), halfSize));
        }

        public bool Any(BoundsInt bounds)
        {
            if (!IntersectsBounds(this.Bounds, bounds))
            {
                return false;
            }

            foreach (var kv in values)
            {
                if (bounds.Contains(kv.Key))
                {
                    return true;
                }
            }

            foreach (var child in children)
            {
                if (child.Any(bounds))
                {
                    return true;
                }
            }

            return false;
        }

        public List<T> Search(BoundsInt bounds)
        {
            var results = new List<T>();
            Search(bounds, results);
            return results;
        }

        public List<T> Find(Vector3Int from, int radius)
        {
            var s = new Vector3Int(radius, radius, radius);
            var b = new BoundsInt(from - s, s * 2);
            var results = Search(b);
            return results.Count > 0 ? results : new List<T>();
        }

        private static bool IntersectsBounds(BoundsInt a, BoundsInt b)
        {
            return (a.min.x >= b.min.x || a.min.x < b.max.x) &&
                (a.min.y >= b.min.y || a.min.y < b.max.y) &&
                (a.min.z >= b.min.z || a.min.z < b.max.z);
        }

        private void Search(BoundsInt bounds, ICollection<T> results)
        {
            if (!IntersectsBounds(this.Bounds, bounds))
            {
                return;
            }

            foreach (var kv in values)
            {
                if (bounds.Contains(kv.Key))
                {
                    results.Add(kv.Value);
                }
            }

            if (children.Count <= 0) return;
            
            foreach (var child in children)
            {
                child.Search(bounds, results);
            }
        }

        public bool Remove(Vector3Int coord)
        {
            if (!Bounds.Contains(coord))
            {
                return false;
            }

            if (values.ContainsKey(coord))
            {
                values.Remove(coord);
                return true;
            }

            foreach (var child in children)
            {
                if (child.Remove(coord))
                {
                    return true;
                }
            }

            return false;
        }
    }
}