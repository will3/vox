using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox
{
    class DesignationMap
    {
        static DesignationMap instance;

        public static DesignationMap Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DesignationMap();
                }
                return instance;
            }
        }

        Octree<Designation> map;

        public DesignationMap() {
            map = new Octree<Designation>(TerrianConfig.Instance.BoundsInt);
        }

        public void AddDesignation(Designation designation)
        {
            map.Add(designation.center, designation);
        }

        public IEnumerable<Designation> Find(Vector3Int from, DesignationType type) {
            int maxTry = 10;

            int radius = 32;

            for (var i = 0; i < maxTry; i++) {
                var results = Find(from, type, radius);
                if (results.Any()) {
                    return results;
                }
            }

            return new List<Designation>();
        }

        public IEnumerable<Designation> Find(Vector3Int from, DesignationType type, int radius) {
            var size = new Vector3Int(radius, radius, radius);
            var bounds = new BoundsInt(from - size, size * 2);
            var results = map.Search(bounds).Where((Designation arg1) => arg1.type == type);

            return results;
        }
    }
}