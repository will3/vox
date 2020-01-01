using System.Collections.Generic;
using System.Linq;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class QuadTree<T>
    {
        private readonly int _size;

        private readonly Dictionary<Vector3Int, QuadTreeChunk> _chunks =
            new Dictionary<Vector3Int, QuadTreeChunk>();

        public QuadTree(int size)
        {
            _size = size;
        }

        private Vector3Int GetOrigin(Vector3Int position)
        {
            return new Vector3Int(
                FloorForOrigin(position.x),
                FloorForOrigin(position.y),
                FloorForOrigin(position.z));
        }

        private int FloorForOrigin(int num)
        {
            return Mathf.FloorToInt(num / (float) _size) * _size;
        }

        private QuadTreeChunk GetOrCreateChunk(Vector3Int origin)
        {
            if (_chunks.TryGetValue(origin, out var chunk))
            {
                return chunk;
            }

            chunk = new QuadTreeChunk();
            _chunks[origin] = chunk;

            return chunk;
        }

        private QuadTreeChunk GetChunk(Vector3Int origin)
        {
            return _chunks.TryGetValue(origin, out var chunk) ? chunk : null;
        }

        public void Add(Vector3Int position, T obj)
        {
            Add(new BoundsInt(position, new Vector3Int(1, 1, 1)), obj);
        }

        public void Add(BoundsInt bounds, T obj)
        {
            var position = bounds.position;
            var origin = GetOrigin(position);
            var chunk = GetOrCreateChunk(origin);
            chunk.Add(bounds, obj);
        }

        public IEnumerable<T> Search(Bounds bounds)
        {
            var min = bounds.min.CeilToInt();
            var max = bounds.max.FloorToInt();

            var boundsInt = new BoundsInt(min, max - min + Vector3Int.one);
            return Search(boundsInt);
        }

        public IEnumerable<T> Search(BoundsInt bounds)
        {
            var origin = GetOrigin(bounds.position);

            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        var o = origin + new Vector3Int(_size * i, _size * j, _size * k);

                        var chunk = GetChunk(o);

                        if (chunk == null)
                        {
                            continue;
                        }

                        var results = chunk.Search(bounds);
                        foreach (var result in results)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        private class QuadTreeChunk
        {
            private readonly Dictionary<BoundsInt, HashSet<T>> _map =
                new Dictionary<BoundsInt, HashSet<T>>();

            public void Add(BoundsInt bounds, T obj)
            {
                if (!_map.TryGetValue(bounds, out var set))
                {
                    set = new HashSet<T>();
                    _map[bounds] = set;
                }

                set.Add(obj);
            }

            public IEnumerable<T> Search(BoundsInt bounds)
            {
                return from kv in _map
                    let b = kv.Key
                    where b.Intersects(bounds)
                    from item in kv.Value
                    select item;
            }
        }
    }

    public static class BoundsIntExtensions
    {
        public static bool Intersects(this BoundsInt a, BoundsInt b)
        {
            if (a.x + a.size.x <= b.x) return false;
            if (a.y + a.size.y <= b.y) return false;
            if (a.z + a.size.z <= b.z) return false;

            if (a.x >= b.x + b.size.x) return false;
            if (a.y >= b.y + b.size.y) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (a.z >= b.z + b.size.z) return false;

            return true;
        }
    }

    public static class BoundsHelper
    {
        public static BoundsInt CalcBounds(IEnumerable<Vector3Int> coords)
        {
            var cs = coords.ToArray();
            var minX = cs.Min(v => v.x);
            var minY = cs.Min(v => v.y);
            var minZ = cs.Min(v => v.z);
            var maxX = cs.Max(v => v.x);
            var maxY = cs.Max(v => v.y);
            var maxZ = cs.Max(v => v.z);

            var min = new Vector3Int(minX, minY, minZ);
            var max = new Vector3Int(maxX, maxY, maxZ);

            return new BoundsInt(min, max - min + new Vector3Int(1, 1, 1));
        }

        public static BoundsInt CalcBounds(Vector3Int a, Vector3Int b)
        {
            var ax = a.x < b.x;
            var ay = a.y < b.y;
            var az = a.z < b.z;

            var min = new Vector3Int(
                ax ? a.x : b.x,
                ay ? a.y : b.y,
                az ? a.z : b.z);

            var max = new Vector3Int(
                ax ? b.x : a.x,
                ay ? b.y : a.y,
                az ? b.z : a.z);

            var size = max - min + Vector3Int.one;

            return new BoundsInt(min, size);
        }
    }
}