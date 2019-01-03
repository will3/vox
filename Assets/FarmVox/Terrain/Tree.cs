using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class Tree
    {
        public readonly HashSet<Vector3Int> TrunkCoords;
        public readonly HashSet<Vector3Int> StumpCoords;
        public readonly Vector3Int Pivot;

        public Tree(HashSet<Vector3Int> stumpCoords, HashSet<Vector3Int> trunkCoords, Vector3Int pivot)
        {
            StumpCoords = stumpCoords;
            TrunkCoords = trunkCoords;
            Pivot = pivot;
        }
    }
}