using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class MeshResult
    {
        public MeshResult(List<VoxelData> voxelData, Mesh mesh)
        {
            VoxelData = voxelData;
            Mesh = mesh;
        }

        public List<VoxelData> VoxelData { get; private set; }
        public Mesh Mesh { get; private set; }
    }
}