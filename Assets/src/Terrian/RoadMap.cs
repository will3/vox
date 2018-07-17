using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FarmVox
{
    public class RoadMap
    {
        class Block {
            Vector3Int coord;
            public Block(Vector3Int coord) {
                this.coord = coord;
            }
        }

        private Dictionary<Vector3Int, Block> blocks = new Dictionary<Vector3Int, Block>();

        private HashSet<Vector3Int> coords = new HashSet<Vector3Int>();

        public HashSet<Vector3Int> Coords
        {
            get
            {
                return coords;
            }
        }

        private Dictionary<Vector2Int, List<Vector3Int>> xzMap = new Dictionary<Vector2Int, List<Vector3Int>>();
        private Dictionary<Vector3Int, float> distances = new Dictionary<Vector3Int, float>();

        public Vector3Int? GetClosestCoord() {
            var minDistance = Mathf.Infinity;
            Vector3Int? minCoord = null;

            foreach(var coord in coords) {
                var distance = distances[coord];
                if (distance < minDistance) {
                    minDistance = distance;
                    minCoord = coord;
                }
            }

            return minCoord;
        }

        public void AddNode(Vector3Int coord, float distance)
        {
            coords.Add(coord);
            var xz = new Vector2Int(coord.x, coord.z);
            if (!xzMap.ContainsKey(xz)) {
                xzMap[xz] = new List<Vector3Int>();
            }
            xzMap[xz].Add(coord);
            distances[coord] = distance;
        }

        public float GetDistance(Vector3Int coord) {
            return distances[coord];
        }

        public bool HasXZ(Vector2Int xz) {
            return xzMap.ContainsKey(xz);
        }

        public void RemoveXZ(Vector2Int xz) {
            if (!xzMap.ContainsKey(xz)) {
                return;
            }

            var list = xzMap[xz];
            foreach(var coord in list) {
                coords.Remove(coord);
            }
            xzMap.Remove(xz);
        }
    }
}