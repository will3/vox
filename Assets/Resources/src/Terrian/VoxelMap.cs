using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public struct Voxel {
        public float value;
        public Color color;
        public VoxelType type;
    }

    public enum VoxelType {
        Air,
        Rock,
        Grass,
        Stone
    }

    public class VoxelMap
    {
        static VoxelMap instance;
        public static VoxelMap Instance {
            get {
                if (instance == null) {
                    instance = new VoxelMap(TerrianConfig.Instance.BoundsInt);
                }
                return instance;
            }
        }

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