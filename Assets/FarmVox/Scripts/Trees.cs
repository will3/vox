using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using FarmVox.Workers;
using UnityEngine;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(Chunks))]
    public class Trees : MonoBehaviour
    {
        public Chunks chunks;
        private TreeMap _treeMap;
        public GameObject treePrefab;
        public TreeConfig config;

        private void Start()
        {
            if (chunks == null)
            {
                chunks = GetComponent<Chunks>();
            }
        }

        public void GenerateTrees(Terrian terrian)
        {
            var boundsInt = terrian.Config.BoundsInt;

            _treeMap = new TreeMap(boundsInt);

            var queue = GameController.Instance.Queue;

            terrian.VisitChunks(chunk =>
            {
                queue.Enqueue(new ActionWorker(() => { GenerateTrees(terrian, chunk); }));
            });
        }

        private void GenerateTrees(Terrian terrian, TerrianChunk terrianChunk)
        {
            var terrianConfig = terrian.Config;
            var defaultLayer = terrian.defaultLayer;

            var treeNoise = config.noise;

            var pine = new PineObject(3.0f, 10, 2);

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;
                var normal = kv.Value;

                // Cannot be stored in tree map
                if (localCoord.x >= terrianConfig.Size || localCoord.y >= terrianConfig.Size ||
                    localCoord.z >= terrianConfig.Size)
                {
                    continue;
                }

                var j = localCoord.y;

                var globalCoord = localCoord + chunk.origin + new Vector3Int(0, 1, 0);
                var noise = (float) treeNoise.GetValue(globalCoord);
                var treeDensity = config.densityFilter.GetValue(noise);

                if (config.random.NextDouble() * treeDensity > 0.02)
                {
                    continue;
                }

                var relY = j + chunk.origin.y - terrianConfig.GroundHeight;

                if (relY <= terrianConfig.WaterLevel)
                {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                var height = relY / terrianConfig.MaxHeight;
                var treeHeightValue = config.heightGradient.GetValue(height);

                var value = noise * treeHeightValue;

                if (value < config.threshold)
                {
                    continue;
                }

                var radius = terrianConfig.TreeMinDis;
                var treeBoundsSize = new Vector3Int(radius, radius, radius);
                var treeBounds = new BoundsInt(globalCoord - treeBoundsSize, treeBoundsSize * 2);
                if (_treeMap.HasTrees(treeBounds))
                {
                    continue;
                }

                var tree = pine.Place(chunks, globalCoord, config);

                var treeGo = Instantiate(treePrefab, transform);
                treeGo.transform.position = globalCoord + new Vector3(1.5f, 0, 1.5f);

                _treeMap.AddTree(tree);
            }

            var treeChunk = chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }
        }
    }
}