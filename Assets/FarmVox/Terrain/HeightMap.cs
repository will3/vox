using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class HeightMap
    {
        readonly Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
        
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

        private const int TileSize = 6;
        
        static Vector2Int GetTileOrigin(Vector3Int coord) {
            var origin = new Vector2Int(
                Mathf.FloorToInt(coord.x / (float)TileSize) * TileSize,
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
            private Vector2Int _origin;
            public readonly int Size;
            
            public Tile(Vector2Int origin, int size)
            {
                _origin = origin;
                Size = size;
            }

            public readonly HashSet<Vector3Int> Coords = new HashSet<Vector3Int>();

            public Vector3Int? Center { get; private set; }
            public bool CanBuild { get; private set; }

            public void AddCoord(Vector3Int coord)
            {
                Coords.Add(coord);
            }

            public void Update()
            {
                CanBuild = true;

                var halfSize = Mathf.CeilToInt(Size / 2.0f);

                // Find center
                Center = null;
                foreach (var coord in Coords)
                {
                    if (coord.x == _origin.x + halfSize &&
                        coord.z == _origin.y + halfSize)
                    {
                        Center = coord;
                        break;
                    } 
                }
                
//                var minY = Mathf.Infinity;
//                var maxY = -Mathf.Infinity;
//
//                var hasAllCoords = true;
//                for (var i = 0; i < Size; i++)
//                {
//                    for (var j = 0; j < Size; j++)
//                    {
//                        var coord = new Vector2Int(_origin.x + i, _origin.z + j);
//                        if (!Coords.ContainsKey(coord))
//                        {
//                            hasAllCoords = false;
//                            continue;
//                        }
//
//                        var pos = Coords[coord];
//                        if (pos.y < minY)
//                        {
//                            minY = pos.y;
//                        }
//
//                        if (pos.y > maxY)
//                        {
//                            maxY = pos.y;
//                        }
//                    }
//                }
//
//                CanBuild = hasAllCoords; // && maxY - minY <= 2;
//
//                var centerXz = new Vector2Int(
//                    _origin.x + Mathf.CeilToInt(Size / 2.0f), 
//                    _origin.z + Mathf.CeilToInt(Size / 2.0f));
//
//                if (Coords.ContainsKey(centerXz))
//                {
//                    Center = Coords[centerXz];    
//                }
            }
        }
    }
}