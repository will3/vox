using System.Collections.Generic;
using System.Linq;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTiles : MonoBehaviour
    {
        private readonly QuadTree<BuildingTile> _tiles = new QuadTree<BuildingTile>(32);
        public GameObject buildingTilePrefab;
        public Terrian terrian;
        public int gridSize = 3;
        private const int SearchVerticalRange = 8;

        public bool TryFind2By2(Vector3 coord, int maxYDis, out IEnumerable<BuildingTile> tiles)
        {
//            var dir = Camera.main.transform.forward * -1;
//            var cameraXDir = dir.x > 0 ? 1 : -1;
//            var cameraZDir = dir.z > 0 ? 1 : -1;

            var dirs = new[]
            {
                new Vector2Int(-1, -1),
                new Vector2Int(1, -1),
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
            };

            var xz = coord.GetXz();

            var a = GetOrCreateBuildingTile(coord);
            var tileXz = a.GetCenterXz();

            var results = dirs.OrderBy(d =>
            {
                var center = tileXz + d * gridSize;
                return (center - xz).sqrMagnitude;
            });

            foreach (var result in results)
            {
                if (TryFind2By2(a, result.x, result.y, out tiles))
                {
                    return true;
                }
            }

            tiles = null;
            return false;
        }

        private bool TryFind2By2(BuildingTile a, int xDir, int zDir, out IEnumerable<BuildingTile> tiles)
        {
            var b = GetOrCreateBuildingTile(a, xDir, 0);
            if (b.hasBuilding)
            {
                tiles = null;
                return false;
            }

            var c = GetOrCreateBuildingTile(a, xDir, zDir);
            if (c.hasBuilding)
            {
                tiles = null;
                return false;
            }
            
            var d = GetOrCreateBuildingTile(a, 0, zDir);
            if (d.hasBuilding)
            {
                tiles = null;
                return false;
            }
            
            tiles = new []
            {
                a, b, c, d
            };

            return true;
        }

        public IEnumerable<BuildingTile> Search(Vector3 position, float radius)
        {
            var size = new Vector3(radius * 2, SearchVerticalRange, radius * 2);
            var bounds = new Bounds(position, size);
            return _tiles.Search(bounds);
        }
        
        public BuildingTile GetOrCreateBuildingTile(Vector3 coord)
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

        public BuildingTile GetOrCreateBuildingTile(BuildingTile tile, int i, int k)
        {
            var next = tile.bounds.center.FloorToInt() + new Vector3Int(i, 0, k) * gridSize;
            return GetOrCreateBuildingTile(next);
        }
        
        private IEnumerable<Vector3Int> CalcBuildingTile(Vector3 coord)
        {
            var xz = GetGridXz(coord);
            var fromY = Mathf.FloorToInt(coord.y);

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
        
        private Vector2Int GetGridXz(Vector3 coord)
        {
            var startX = Mathf.FloorToInt(coord.x / gridSize) * gridSize;
            var startZ = Mathf.FloorToInt(coord.z / gridSize) * gridSize;
            return new Vector2Int(startX, startZ);
        }
    }
}