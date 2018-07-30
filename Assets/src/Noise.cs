using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Noise
    {
        public float frequency = 0.01f;
        public float amplitude = 1.0f;
        public int seed = 1337;
        public float lacunarity = 2.0f;
        public float persistence = 0.5f;
        public int octaves = 5;
        public float yScale = 1.0f;
        public float xzScale = 1.0f;
    }

    public static class BoundCoords
    {
        public static IEnumerator<Vector3Int> LoopCoords(Bounds bounds) {
            for (var i = bounds.min.x; i < bounds.max.x; i++) 
            {
                for (var j = bounds.min.y; j < bounds.max.y; j++)
                {
                    for (var k = bounds.min.z; k < bounds.max.z; k++)
                    {
                        var coord = new Vector3Int(
                            Mathf.FloorToInt(i),
                            Mathf.FloorToInt(j),
                            Mathf.FloorToInt(k));
                        yield return coord;
                    }
                }   
            }
        }
    }
}