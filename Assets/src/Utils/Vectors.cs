﻿using UnityEngine;

namespace FarmVox
{
    public class Vectors
    {
        public static Vector3Int GetVector3Int(int i, int j, int k, int d)
        {
            if (d == 0)
            {
                return new Vector3Int(i, j, k);
            }
            else if (d == 1)
            {
                return new Vector3Int(k, i, j);
            }
            return new Vector3Int(j, k, i);
        }
    }
}