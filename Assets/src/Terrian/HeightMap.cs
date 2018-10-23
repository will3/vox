using System.Collections.Generic;
using System.Xml.Xsl;
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

            var tilesToUpdate = new List<Tile>();
            
            foreach (var coord in chunk.SurfaceCoordsUp)
            {
                var worldCoord = coord + chunk.Origin;
                var tile = GetOrCreateTile(worldCoord);

                tile.AddCoord(worldCoord);
                tilesToUpdate.Add(tile);
            }

            foreach (var tile in tilesToUpdate)
            {
                tile.Update();    
            }
        }

        public Tile GetTile(Vector3Int coord) {
            var origin = GetTileOrigin(coord);
            Tile tile;
            _tiles.TryGetValue(origin, out tile);
            return tile;
        }

        const int TileSize = 7;
        
        static Vector3Int GetTileOrigin(Vector3Int coord) {
            var origin = new Vector3Int(
                Mathf.FloorToInt(coord.x / (float)TileSize) * TileSize,
                Mathf.FloorToInt(coord.y / (float)TileSize) * TileSize,
                Mathf.FloorToInt(coord.z / (float)TileSize) * TileSize
            );

            return origin;
        }

        private Tile GetOrCreateTile(Vector3Int coord)
        {
            var origin = GetTileOrigin(coord);

            if (!_tiles.ContainsKey(origin))
            {
                _tiles[origin] = new Tile(origin, TileSize);
            }

            return _tiles[origin];
        }

        public class Tile
        {
            private Vector3Int _origin;
            public readonly int Size;
            
            public Tile(Vector3Int origin, int size)
            {
                _origin = origin;
                Size = size;
            }

            // There's a bug here, 2 coords can share the same xz
            public readonly Dictionary<Vector2Int, Vector3Int> Coords = new Dictionary<Vector2Int, Vector3Int>();

            public Vector3Int Center { get; private set; }
            public bool CanBuild { get; private set; }

            public void AddCoord(Vector3Int coord)
            {
                Coords[new Vector2Int(coord.x, coord.z)] = coord;
            }

            public void Update()
            {
                var minY = Mathf.Infinity;
                var maxY = -Mathf.Infinity;
                
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        var coord = new Vector2Int(_origin.x + i, _origin.z + j);
                        if (!Coords.ContainsKey(coord))
                        {
                            CanBuild = false;
                            break;
                        }

                        var pos = Coords[coord];
                        if (pos.y < minY)
                        {
                            minY = pos.y;
                        }

                        if (pos.y > maxY)
                        {
                            maxY = pos.y;
                        }
                    }
                }

                CanBuild = maxY - minY <= 2;

                var centerXz = new Vector2Int(
                    _origin.x + Mathf.CeilToInt(Size / 2.0f), 
                    _origin.z + Mathf.CeilToInt(Size / 2.0f));
                Center = Coords[centerXz];
            }
        }
    }
}