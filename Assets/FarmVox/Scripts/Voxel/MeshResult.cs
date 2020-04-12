using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class MeshResult
    {
        public MeshResult(List<VoxelData> voxelData, Mesh mesh)
        {
            VoxelData = voxelData;
            Mesh = mesh;
        }

        public List<VoxelData> VoxelData { get; }
        public Mesh Mesh { get; }
    }
}