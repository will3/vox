using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianColumnDistanceComparer : IComparer<TerrianColumn>
    {
        public int Compare(TerrianColumn x, TerrianColumn y)
        {
            return x.Distance.CompareTo(y.Distance);
        }
    }
}