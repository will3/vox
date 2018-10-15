using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class HeightMap
    {
        Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

        public void LoadChunk(Terrian terrian, TerrianChunk terrianChunk)
        {
            var chunk = terrian.DefaultLayer.GetChunk(terrianChunk.Origin);

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.surfaceCoordsUp)
            {
                var worldCoord = coord + chunk.Origin;
                var tile = GetOrCreateTile(worldCoord);

                tile.AddCoord(worldCoord);
            }
        }

        public Tile GetTile(Vector3Int coord) {
            var origin = GetTileOrigin(coord);
            Tile tile = null;
            tiles.TryGetValue(origin, out tile);
            return tile;
        }

        Vector3Int GetTileOrigin(Vector3Int coord) {
            int tileSize = 7;

            var origin = new Vector3Int(
                Mathf.FloorToInt(coord.x / (float)tileSize) * tileSize,
                Mathf.FloorToInt(coord.y / (float)tileSize) * tileSize,
                Mathf.FloorToInt(coord.z / (float)tileSize) * tileSize
            );

            return origin;
        }

        Tile GetOrCreateTile(Vector3Int coord)
        {
            var origin = GetTileOrigin(coord);

            if (!tiles.ContainsKey(origin))
            {
                tiles[origin] = new Tile(origin);
            }

            return tiles[origin];
        }

        public class Tile
        {
            Vector3Int origin;
            public Tile(Vector3Int origin)
            {
                this.origin = origin;
            }

            // There's a bug here, 2 coords can be in the same cube
            Dictionary<Vector2Int, Vector3Int> coords = new Dictionary<Vector2Int, Vector3Int>();

            public void AddCoord(Vector3Int coord)
            {
                coords[new Vector2Int(coord.x, coord.z)] = coord;
            }

            public bool CanBuild() {
                var minY = Mathf.Infinity;
                var maxY = -Mathf.Infinity;
                foreach(var value in coords.Values) {
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