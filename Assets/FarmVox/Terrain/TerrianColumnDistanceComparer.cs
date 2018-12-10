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

        float GetDistance(TerrianColumn column) {
            var xDis = column.Origin.x + column.Size / 2.0f;
            var zDis = column.Origin.z + column.Size / 2.0f;

            var distance = (Mathf.Abs(xDis) + Mathf.Abs(zDis)) * 1024;

            return distance;
        }
    }
}