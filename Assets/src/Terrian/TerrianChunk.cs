using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FarmVox
{
    public partial class TerrianChunk
    {
        private readonly HashSet<int> waters = new HashSet<int>();
        private readonly Routes routes = new Routes();

        public Routes Routes
        {
            get
            {
                return routes;
            }
        }

        public readonly Vector3Int key;
        public bool rockNeedsUpdate = true;
        public bool waterNeedsUpdate = true;
        public bool grassNeedsUpdate = true;
        public bool treesNeedsUpdate = true;
        public bool shadowsNeedsUpdate = true;
        public bool housesNeedsUpdate = true;
        public bool routesNeedsUpdate = true;
        public bool growthNeedsUpdate = true;
        public bool townPointsNeedsUpdate = true;
        public bool roadsNeedsUpdate = true;

        public Terrian Terrian;

        private int distance;
        private Vector3Int origin;
        private int waterLevel = 2;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        public Chunk Chunk;
        private readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();

        private int size;

        public int Distance
        {
            get
            {
                return distance;
            }
        }

        public TerrianChunk(Vector3Int key, int size)
        {
            this.key = key;
            this.origin = key * size;
            this.size = size;
            this.dataSize = size + 3;
        }

        public readonly int dataSize;

        public void UpdateDistance(int x, int z)
        {
            var xDis = Mathf.Abs(x - this.key.x);
            var zDis = Mathf.Abs(z - this.key.z);
            distance = Mathf.Max(xDis, zDis);
        }

        public void SetTree(Vector3Int coord, bool flag)
        {
            if (flag)
            {
                trees.Add(coord);
            }
            else
            {
                trees.Remove(coord);
            }
        }

        public bool GetTree(Vector3Int coord)
        {
            return trees.Contains(coord);
        }

        public void SetWater(int i, int j, int k, bool flag)
        {
            var index = getIndex(i, j, k);
            if (flag)
            {
                waters.Add(index);
            }
            else
            {
                waters.Remove(index);
            }
        }

        public bool GetWater(Vector3Int coord) {
            return GetWater(coord.x, coord.y, coord.z);
        }

        public bool GetWater(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            return waters.Contains(index);
        }

        private int getIndex(int i, int j, int k)
        {
            int index = i * dataSize * dataSize + j * dataSize + k;
            return index;
        }

        public float GetOtherTrees(Vector3Int from)
        {
            float min = 5.0f;
            float sqrMin = min * min;
            var amount = 0f;
            foreach (var coord in trees)
            {
                var sqrDis = (coord - from).sqrMagnitude;
                if (sqrDis < sqrMin)
                {
                    amount += 1.0f / sqrDis;
                }
            }

            return amount;
        }

        public void UpdateRoutes()
        {
            if (!routesNeedsUpdate)
            {
                return;
            }
            routes.Clear();
            routes.LoadChunk(Chunk);
            routesNeedsUpdate = false;
        }

        public void DrawRoutesGizmos()
        {
            Gizmos.color = Color.red;
            var offset = new Vector3(0.5f, 1.5f, 0.5f);
            foreach (var kv in routes.Map)
            {
                var from = kv.Key + offset;
                foreach (var b in kv.Value)
                {
                    var to = b.node + offset;
                    Gizmos.DrawLine(from, to);
                }
            }
        }

        public void GenerateWaters()
        {
            if (!waterNeedsUpdate)
            {
                return;
            }

            var chunk = Chunk;
            if (chunk.Origin.y < waterLevel)
            {
                float maxJ = waterLevel - chunk.Origin.y;
                if (maxJ > chunk.Size)
                {
                    maxJ = chunk.Size;
                }
                for (var i = 0; i < chunk.dataSize; i++)
                {
                    for (var k = 0; k < chunk.dataSize; k++)
                    {
                        for (var j = 0; j < maxJ; j++)
                        {
                            if (chunk.Get(i, j, k) <= 0.5)
                            {
                                chunk.Set(i, j, k, 1);
                                chunk.SetColor(i, j, k, Colors.water);
                                SetWater(i, j, k, true);
                            }
                        }
                    }
                }
            }

            waterNeedsUpdate = false;
        }

        readonly HashSet<Vector3Int> townPoints = new HashSet<Vector3Int>();

        public HashSet<Vector3Int> TownPoints
        {
            get
            {
                return townPoints;
            }
        }

        public TerrianConfig Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value;
            }
        }

        public void AddTownPoint(Vector3Int townPoint)
        {
            townPoints.Add(townPoint);
        }

        public HashSet<Routes.Connection> GetConnections(Vector3Int node)
        {
            if (routes.Map.ContainsKey(node)) {
                return routes.Map[node];
            }

            var origin = Terrian.getOrigin(node.x, node.y, node.z);
            var terrianChunk = Terrian.GetTerrianChunk(origin);
            if (terrianChunk == null) {
                return new HashSet<Routes.Connection>();
            }

            if (!terrianChunk.routes.Map.ContainsKey(node)) {
                return new HashSet<Routes.Connection>();
            }

            return terrianChunk.routes.Map[node];
        }

        private TerrianConfig config;

        public RoadMap GetRoadMap(Vector3Int node, float maxDis) {
            var roadMap = new RoadMap();
            HashSet<Vector3Int> leads = new HashSet<Vector3Int>();
            var map = new Dictionary<Vector3Int, float>();

            leads.Add(node);
            map[node] = 0;

            while(leads.Count > 0) {
                var currentNode = leads.First();
                var cost = map[currentNode];

                foreach (var connection in GetConnections(currentNode))
                {
                    var connectionCost = connection.cost;
                    if (Terrian.GetWater(connection.node)) {
                        connectionCost = Mathf.Infinity;
                    }
                    var nextCost = cost + connectionCost;
                    if (nextCost > maxDis) {
                        continue;   
                    }

                    if (map.ContainsKey(connection.node))
                    {
                        if (nextCost >= map[currentNode])
                        {
                            continue;
                        }
                    }

                    map[connection.node] = nextCost;
                    leads.Add(connection.node);
                }

                leads.Remove(currentNode);    
            }

            foreach (var kv in map)
            {
                roadMap.AddNode(kv.Key, kv.Value);
            }

            return roadMap;
        }
    }
}