using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    class TerrianColumnDistanceComparer : IComparer<TerrianColumn>
    {
        public int Compare(TerrianColumn x, TerrianColumn y)
        {
            return GetDistance(x).CompareTo(GetDistance(y));
        }

        float GetDistance(TerrianColumn column)
        {
            return column.Distance;
        }
    }
}