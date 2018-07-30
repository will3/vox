using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox
{
    public class StorageMap
    {
        static StorageMap instance;

        public static StorageMap Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StorageMap();
                }
                return instance;
            }
        }

        readonly Octree<StorageBlock> map;

        StorageMap()
        {
            map = new Octree<StorageBlock>(TerrianConfig.Instance.BoundsInt);
        }

        public void Add(StorageBlock block)
        {
            map.Add(block.coord, block);
        }

        public void Remove(Vector3Int coord)
        {
            map.Remove(coord);
        }

        public StorageBlock Find(Vector3Int from, float minStorage)
        {
            int radius = 32;
            int maxTry = 5;

            for (var i = 0; i < maxTry; i++)
            {
                var results = map.Find(from, radius).Where((x) => x.Storage >= minStorage && !x.allocated);

                if (results.Any())
                {
                    return FindClosest(from, results);
                }

                radius *= 2;
            }

            return null;
        }

        StorageBlock FindClosest(Vector3Int from, IEnumerable<StorageBlock> blocks) {
            var minDis = Mathf.Infinity;
            StorageBlock minBlock = null;

            foreach(var block in blocks) {
                var dis = (block.coord - from).sqrMagnitude;
                if (dis < minDis) {
                    minDis = dis;
                    minBlock = block;
                }
            }
            return minBlock;
        }
    }
}