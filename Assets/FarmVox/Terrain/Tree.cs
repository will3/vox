using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class Tree
    {
        public readonly Vector3Int Pivot;

        public Tree(Vector3Int pivot)
        {
            Pivot = pivot;
        }
    }
}