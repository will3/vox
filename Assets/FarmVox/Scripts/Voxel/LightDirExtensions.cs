using System;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public static class LightDirExtensions
    {
        public static Vector3Int GetDirVector(this LightDir dir)
        {
            switch (dir)
            {
                case LightDir.NorthEast:
                    return new Vector3Int(-1, -1, -1);
                case LightDir.SouthEast:
                    return new Vector3Int(-1, -1, 1);
                case LightDir.NorthWest:
                    return new Vector3Int(1, -1, -1);
                case LightDir.SouthWest:
                    return new Vector3Int(1, -1, 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }
}