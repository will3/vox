using UnityEngine;
using System.Collections;

public class Raycast4545
{
    public static Vector3 LightDir = new Vector3(1, 1, 1);
    public static bool Trace(Vector3Int coord, Chunks chunks, int depth = 64) {
        bool first = true;
        for (var i = 0; i < depth; i ++) {
            coord.x += 1;
            coord.z += 1;
            coord.y += 1;
            var value = chunks.Get(coord.x, coord.y, coord.z);
            if (value > 0) {
                if (first) {
                    first = false;
                } else {
                    return true;    
                }
            }
        }

        return false;
    }
}
