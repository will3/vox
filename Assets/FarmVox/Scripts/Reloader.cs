using System.Collections.Generic;
using System.Linq;
using FarmVox.Scripts.Voxel;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Reloader : MonoBehaviour, ICommand
    {
        public int size = 32;
        public Ground ground;
        public Water water;
        public Trees trees;
        public Waterfalls waterfalls;
        public BuildingTiles tiles;

        private readonly HashSet<Vector3Int> _columnsToUnload = new HashSet<Vector3Int>();

        private string _lastConfig;

        public Vector3Int numGridsToGenerate = new Vector3Int(1, 2, 1);
        public int distanceToUnload = 1;

        private void Awake()
        {
            CommandManager.Instance.Add(this);
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

        private void UnloadColumns()
        {
            var copy = new HashSet<Vector3Int>(_columnsToUnload);
            foreach (var column in copy)
            {
                UnloadColumn(column);
            }

            _columnsToUnload.ExceptWith(copy);
        }

        private void UnloadColumn(Vector3Int column)
        {
            foreach (var chunk in GetChunks(column))
            {
                ground.UnloadChunk(chunk);
                trees.UnloadChunk(chunk);
                water.UnloadChunk(chunk);
                waterfalls.UnloadChunk(chunk);
                tiles.UnloadChunk(chunk);
            }
        }

        private IEnumerable<Vector3Int> GetChunks(Vector3Int columnOrigin)
        {
            for (var i = 0; i < numGridsToGenerate.y; i++)
            {
                yield return new Vector3Int(columnOrigin.x, size * i, columnOrigin.z);
            }
        }

        public string CommandName => "reload";

        public string Run(string[] args)
        {
            foreach (var column in ground.Columns)
            {
                _columnsToUnload.Add(column);
            }

            UpdateColumnsToUnload();
            UnloadColumns();

            return "";
        }
    }
}