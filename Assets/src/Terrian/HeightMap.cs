using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class HeightMap
    {
        readonly Dictionary<Vector3Int, Tile> _tiles = new Dictionary<Vector3Int, Tile>();

        public void LoadChunk(Terrian terrian, TerrianChunk terrianChunk)
        {
            var chunk = terrian.DefaultLayer.GetChunk(terrianChunk.Origin);

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.SurfaceCoordsUp)
            {
                var worldCoord = coord + chunk.Origin;
                var tile = GetOrCreateTile(worldCoord);

                tile.AddCoord(worldCoord);
            }
        }

        public Tile GetTile(Vector3Int coord) {
            var origin = GetTileOrigin(coord);
            Tile tile;
            _tiles.TryGetValue(origin, out tile);
            return tile;
        }

        static Vector3Int GetTileOrigin(Vector3Int coord) {
            const int tileSize = 7;

            var origin = new Vector3Int(
                Mathf.FloorToInt(coord.x / (float)tileSize) * tileSize,
                Mathf.FloorToInt(coord.y / (float)tileSize) * tileSize,
                Mathf.FloorToInt(coord.z / (float)tileSize) * tileSize
            );

            return origin;
        }

        private Tile GetOrCreateTile(Vector3Int coord)
        {
            var origin = GetTileOrigin(coord);

            if (!_tiles.ContainsKey(origin))
            {
                _tiles[origin] = new Tile(origin);
            }

            return _tiles[origin];
        }

        public class Tile
        {
            private Vector3Int _origin;
            public Tile(Vector3Int origin)
            {
                _origin = origin;
            }

            // There's a bug here, 2 coords can share the same xz
            public readonly Dictionary<Vector2Int, Vector3Int> Coords = new Dictionary<Vector2Int, Vector3Int>();

            public void AddCoord(Vector3Int coord)
            {
                Coords[new Vector2Int(coord.x, coord.z)] = coord;
            }

            public bool CanBuild() {
                var minY = Mathf.Infinity;
                var maxY = -Mathf.Infinity;
                foreach(var value in Coords.Values) {
                    if (value.y > maxY) {
                        maxY = value.y;
                    }
                    if (value.y < minY) {
                        minY = value.y;
                    }
                }

                return maxY - minY <= 2;
            }
        }
    }
}