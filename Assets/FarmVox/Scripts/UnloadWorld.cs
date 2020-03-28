using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class UnloadWorld : MonoBehaviour
    {
        public int size = 32;
        public Ground ground;
        public Water water;
        public Trees trees;
        public Waterfalls waterfalls;
        public BuildingTiles tiles;

        public float waitForSeconds = 0.1f;

        private readonly HashSet<Vector3Int> _columnsToUnload = new HashSet<Vector3Int>();

        private string _lastConfig;

        public Vector3Int numGridsToGenerate = new Vector3Int(1, 2, 1);
        public int distanceToUnload = 1;

        public BoundsInt Bounds =>
            new BoundsInt
            {
                min = new Vector3Int(-numGridsToGenerate.x, 0, -numGridsToGenerate.z) * size,
                max = new Vector3Int(numGridsToGenerate.x + 1, 0, numGridsToGenerate.z + 1) * size
            };

        private void Start()
        {
            StartCoroutine(UnloadColumnsLoop());
        }

        private IEnumerator UnloadColumnsLoop()
        {
            while (true)
            {
                UpdateColumnsToUnload();
                yield return UnloadColumns();

                yield return new WaitForSeconds(waitForSeconds);
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Return)) return;
            foreach (var column in ground.Columns)
            {
                _columnsToUnload.Add(column);
            }
        }

        private void UpdateColumnsToUnload()
        {
            var playerOrigin = transform.position.GetOrigin(size);
            playerOrigin.y = 0;

            var columnsToUnload = ground.Columns.Where(column =>
            {
                var xDis = Mathf.Abs((playerOrigin.x - column.x) / size);
                var zDis = Mathf.Abs((playerOrigin.z - column.z) / size);
                return Mathf.Max(xDis, zDis) > distanceToUnload;
            }).ToArray();

            _columnsToUnload.UnionWith(columnsToUnload);
        }

        private IEnumerator UnloadColumns()
        {
            var copy = new HashSet<Vector3Int>(_columnsToUnload);
            foreach (var column in copy)
            {
                yield return UnloadColumn(column);
            }

            _columnsToUnload.ExceptWith(copy);
        }

        private IEnumerator UnloadColumn(Vector3Int column)
        {
            foreach (var chunk in GetChunks(column))
            {
                ground.UnloadChunk(chunk);
                trees.UnloadChunk(chunk);
                water.UnloadChunk(chunk);
                waterfalls.UnloadChunk(chunk);
                tiles.UnloadChunk(chunk);

                yield return new WaitForSeconds(waitForSeconds);
            }
        }

        private IEnumerable<Vector3Int> GetChunks(Vector3Int columnOrigin)
        {
            for (var i = 0; i < numGridsToGenerate.y; i++)
            {
                yield return new Vector3Int(columnOrigin.x, size * i, columnOrigin.z);
            }
        }
    }
}