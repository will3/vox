using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    class Octree<T>
    {
        Vector3Int start;
        Vector3Int size;
        Vector3Int end;
        BoundsInt bounds;

        int minSize = 4;
        int maxValues = 32;

        int count;

        public int Count
        {
            get
            {
                return count;
            }
        }

        List<Octree<T>> children = new List<Octree<T>>();
        Dictionary<Vector3Int, T> values = new Dictionary<Vector3Int, T>();

        public delegate void VisitDelegate(Vector3Int coord, T value);

        public void Visit(VisitDelegate visitDelegate)
        {
            foreach (var kv in values)
            {
                visitDelegate(kv.Key, kv.Value);
            }

            foreach (var child in children)
            {
                child.Visit(visitDelegate);
            }
        }

        public Octree(Vector3Int start, Vector3Int size) {
            this.start = start;
            this.size = size;
            bounds = new BoundsInt();
            bounds.min = start;
            bounds.max = start + size;
        }

        public Octree(BoundsInt bounds)
        {
            this.bounds = bounds;
            start = Vectors.FloorToInt(bounds.min);
            size = Vectors.FloorToInt(bounds.size);
        }

        public bool Contains(Vector3Int pos) {
            return bounds.Contains(pos);    
        }

        public bool Add(Vector3Int pos, T value)
        {
            if (!bounds.Contains(pos)) {
                return false;
            }

            bool leaf = values.Count < maxValues || size.x <= minSize;

            count++;

            if (leaf)
            {
                values[pos] = value;
                return true;
            }

            Divide();

            foreach (var child in children)
            {
                if (child.Add(pos, value))
                {
                    return true;
                }
            }

            return false;
        }

        public void Divide()
        {
            if (children.Count > 0)
            {
                return;
            }

            var halfX = size.x / 2;
            var halfY = size.y / 2;
            var halfZ = size.z / 2;

            var halfSize = new Vector3Int(halfX, halfY, halfZ);

            children.Add(new Octree<T>(new Vector3Int(start.x,          start.y,            start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x + halfX,  start.y,            start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x,          start.y + halfY,    start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x + halfX,  start.y + halfY,    start.z), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x,          start.y,            start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x + halfX,  start.y,            start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x,          start.y + halfY,    start.z + halfZ), halfSize));
            children.Add(new Octree<T>(new Vector3Int(start.x + halfX,  start.y + halfY,    start.z + halfZ), halfSize));
        }

        public bool Any(BoundsInt bounds) {
            if (!IntersectsBounds(this.bounds, bounds)) {
                return false;
            }

            foreach (var kv in values) {
                if (bounds.Contains(kv.Key)) {
                    return true;
                }
            }

            foreach (var child in children) {
                if (child.Any(bounds)) {
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

        public List<T> Find(Vector3Int from, int radius) {
            var s = new Vector3Int(radius, radius, radius);
            var b = new BoundsInt(from - s, s * 2);
            var results = Search(b);
            if (results.Count > 0) {
                return results;
            }

            return new List<T>();
        }

        bool IntersectsBounds(BoundsInt a, BoundsInt b) {
            return (a.min.x >= b.min.x || a.min.x < b.max.x) &&
                (a.min.y >= b.min.y || a.min.y < b.max.y) &&
                (a.min.z >= b.min.z || a.min.z < b.max.z);
        }

        public void Search(BoundsInt bounds, List<T> results)
        {
            if (!IntersectsBounds(this.bounds, bounds)) {
                return;
            }

            foreach (var kv in values)
            {
                if (bounds.Contains(kv.Key))
                {
                    results.Add(kv.Value);
                }
            }

            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    child.Search(bounds, results);
                }
            }
        }

        public bool Remove(Vector3Int coord)
        {
            if (!bounds.Contains(coord))
            {
                return false;
            }

            count--;

            if (values.ContainsKey(coord))
            {
                values.Remove(coord);
                return true;
            }
            else
            {
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
}