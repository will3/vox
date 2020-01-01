using System.Collections.Generic;
using System.Linq;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingTiles : MonoBehaviour
    {
        private readonly QuadTree<BuildingTile> _tiles = new QuadTree<BuildingTile>(32);
        public GameObject buildingTilePrefab;
        public Terrian terrian;
        public int gridSize = 3;

        public IEnumerable<BuildingTile> Search(BoundsInt bounds)
        {
            return _tiles.Search(bounds);
        }

        public IEnumerable<BuildingTile> Search(Bounds bounds)
        {
            return _tiles.Search(bounds);
        }
        
        public BuildingTile GetOrCreateBuildingTile(Vector3Int coord)
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
        
        private Vector2Int GetGridXZ(Vector3Int coord)
        {
            var startX = Mathf.FloorToInt(coord.x / (float) gridSize) * gridSize;
            var startZ = Mathf.FloorToInt(coord.z / (float) gridSize) * gridSize;
            return new Vector2Int(startX, startZ);
        }
    }
}