using System.Collections.Generic;
using System.Linq;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BuildingTileTracker : MonoBehaviour
    {
        public Terrian terrian;

        public int gridSize = 3;
        public Actor actor;
        public GameObject buildingTilePrefab;
        public float highlightDistance = 10.0f;
        public int searchRange = 3;
        public float maxOpacity = 0.2f;
        public float highlightPowCurve = 0.4f;

        private Vector3Int _lastCoord;

        private readonly QuadTree<BuildingTile> _tiles = new QuadTree<BuildingTile>(32);

        public BuildingTile CurrentBuildingTile { get; private set; }

        private void Update()
        {
            var coord = GetCoord();

            if (_lastCoord != coord)
            {
                UpdateNearbyTiles(coord);
                HighlightNearbyTiles(coord);
            }

            _lastCoord = coord;
        }

//        private void ShowBuildingGridIfNeeded()
//        {
//            if (!Input.GetKey(KeyCode.Space))
//            {
//                return;
//            }
//
//            var coord = GetCoord();
//
//            var vector = Camera.main.transform.position - actor.transform.position;
//            vector.Normalize();
//            var dir = Vector3.ProjectOnPlane(vector, Vector3.up);
//            var xDir = dir.x > 0 ? 1 : -1;
//            var zDir = dir.z > 0 ? 1 : -1;
//            var xz = GetGridXZ(coord);
//
//            const int yRange = 10;
//
//            var a = new Vector3Int(xz.x, coord.y - yRange, xz.y);
//            var b = a + new Vector3Int(xDir * gridSize, yRange * 2, zDir * gridSize);
//            var bounds = BoundsHelper.CalcBounds(a, b);
//
//            var found = _tiles.Search(bounds);
//
//            foreach (var tile in found)
//            {
//                tile.SetHighlightAmount(0.4f);
//            }
//        }

        private Vector3Int GetCoord()
        {
            var position = actor.transform.position;
            return new Vector3Int(
                Mathf.FloorToInt(position.x),
                Mathf.FloorToInt(position.y),
                Mathf.FloorToInt(position.z));
        }

        private void UpdateNearbyTiles(Vector3Int coord)
        {
            CurrentBuildingTile = GetOrCreateBuildingTile(coord);

            for (var i = -searchRange; i <= searchRange; i++)
            {
                for (var k = -searchRange; k <= searchRange; k++)
                {
                    if (i == 0 && k == 0)
                    {
                        continue;
                    }

                    var next = coord + new Vector3Int(i, 0, k) * gridSize;
                    GetOrCreateBuildingTile(next);
                }
            }
        }

        private void HighlightNearbyTiles(Vector3Int coord)
        {
            var center = coord + new Vector3(0.5f, 0.5f, 0.5f);
            var size = new Vector3(20, 8, 20);
            var bounds = new Bounds(center, size);
            var tiles = _tiles.Search(bounds);

            foreach (var tile in tiles)
            {
                var distance = (center - tile.bounds.center).magnitude;
                var r = Mathf.Clamp01(1 - distance / highlightDistance);
                r = Mathf.Pow(r, highlightPowCurve);
                var amount = r * maxOpacity;
                tile.SetHighlightAmount(amount);
            }
        }

        private BuildingTile GetOrCreateBuildingTile(Vector3Int coord)
        {
            var coords = CalcBuildingTile(coord).ToArray();

            if (!coords.Any())
            {
                return null;
            }

            var bounds = BoundsHelper.CalcBounds(coords);

            var tile = _tiles.Search(bounds).FirstOrDefault();
            if (tile != null)
            {
                return tile;
            }

            var tileGo = Instantiate(buildingTilePrefab, transform);
            tile = tileGo.GetComponent<BuildingTile>();
            // TODO maybe pass ground data in
            var mesher = tileGo.GetComponent<BuildingTileMesher>();
            mesher.terrian = terrian;
            tile.coords = coords;
            tile.bounds = bounds;

            _tiles.Add(bounds, tile);

            return tile;
        }

        private Vector2Int GetGridXZ(Vector3Int coord)
        {
            var startX = Mathf.FloorToInt(coord.x / (float) gridSize) * gridSize;
            var startZ = Mathf.FloorToInt(coord.z / (float) gridSize) * gridSize;
            return new Vector2Int(startX, startZ);
        }

        private IEnumerable<Vector3Int> CalcBuildingTile(Vector3Int coord)
        {
            var xz = GetGridXZ(coord);
            var fromY = coord.y;

            var tiles = new HashSet<Vector3Int>();

            for (var i = 0; i < gridSize; i++)
            {
                for (var k = 0; k < gridSize; k++)
                {
                    var c = new Vector3Int(xz.x + i, fromY, xz.y + k);
                    var result = SearchUp(c, 10) ??
                                 SearchDown(c, 10);

                    if (result == null)
                    {
                        continue;
                    }

                    tiles.Add(result.Value);
                }
            }

            return tiles;
        }

        private Vector3Int? SearchDown(Vector3Int coord, int searchDistance)
        {
            for (var i = 0; i < searchDistance; i++)
            {
                var next = coord + Vector3Int.down * i;
                var hasGround = terrian.IsGround(next);

                if (hasGround)
                {
                    return next;
                }
            }

            return null;
        }

        private Vector3Int? SearchUp(Vector3Int coord, int searchDistance)
        {
            for (var i = 0; i < searchDistance; i++)
            {
                var next = coord + Vector3Int.up * i;
                var hasGround = terrian.IsGround(next);

                if (i == 0 && !hasGround)
                {
                    return null;
                }

                if (hasGround)
                {
                    continue;
                }

                return next + Vector3Int.down;
            }

            return null;
        }
    }
}