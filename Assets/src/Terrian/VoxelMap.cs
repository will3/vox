using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public struct Voxel {
        float value;
        Color color;
        int type;
    }

    public class VoxelMap
    {
        Octree<Voxel> tree;

        BoundsInt bounds;

        public VoxelMap(BoundsInt bounds) {
            this.bounds = bounds;
            tree = new Octree<Voxel>(bounds);
        }

        public void Set(Vector3Int coord, Voxel voxel) {
            tree.Set(coord, voxel);
        }

        public void Remove(Vector3Int coord) {
            tree.Remove(coord);
        }
    }
}