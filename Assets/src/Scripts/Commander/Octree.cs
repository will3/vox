using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    class Octree<T>
    {
        Vector3Int start;
        Vector3Int size;
        Vector3Int end;
        Bounds bounds;

        int minSize = 4;
        int maxValues = 10;

        HashSet<Octree<T>> children = new HashSet<Octree<T>>();
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

        public Octree(Vector3Int start, Vector3Int size)
        {
            this.start = start;
            this.size = size;
            end = start + size;
            bounds = new Bounds();
            bounds.min = start;
            bounds.max = end;
        }

        public bool Add(Vector3Int pos, T value)
        {
            if (!bounds.Contains(pos)) {
                return false;
            }

            bool leaf = values.Count < maxValues || size.x <= minSize;

            if (leaf)
            {
                values[pos] = value;
                return true;
            }
            else
            {
                Divide();
                foreach (var child in children)
                {
                    if (child.Add(pos, value))
                    {
                        return true;
                    }
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

        public HashSet<T> Search(Bounds bounds)
        {
            var results = new HashSet<T>();
            Search(bounds, results);
            return results;
        }

        public void Search(Bounds bounds, HashSet<T> results)
        {
            if (!this.bounds.Intersects(bounds))
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