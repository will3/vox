using UnityEngine;
using System.Collections.Generic;

namespace FarmVox
{
    public static class Raycast4545
    {
        public static Vector3 LightDir = new Vector3(1, 1, 1).normalized;

        public static Vector3Int? Trace(Vector3Int coord, IList<Chunks> chunksList, int depth = 64)
        {
            for (var i = 0; i < depth; i++)
            {
                coord.x -= 1;
                coord.y -= 1;
                coord.z -= 1;

                foreach(var chunks in chunksList)
                {
                    var value = chunks.Get(coord.x, coord.y, coord.z);
                    if (value > 0) {
                        return coord;
                    }
                }
            }

            return null;
        }
    }
}