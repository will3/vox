using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class BoundCoords
    {
        public static IEnumerator<Vector3Int> LoopCoords(BoundsInt bounds)
        {
            for (var i = bounds.min.x; i < bounds.max.x; i++)
            {
                for (var j = bounds.min.y; j < bounds.max.y; j++)
                {
                    for (var k = bounds.min.z; k < bounds.max.z; k++)
                    {
                        var coord = new Vector3Int(i, j, k);
                        yield return coord;
                    }
                }
            }
        }
    }
}