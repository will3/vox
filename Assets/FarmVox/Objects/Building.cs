using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Objects
{
    public class Building
    {
        public string Name { get; set; }

        public string ModelName { get; set; }

        public HeightMap.Tile Tile { get; set; }
        
        public Vector3Int Offset { get; set; }
    }
}