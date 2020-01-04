using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class MeshResult
    {
        public MeshResult(List<CoordData> voxelData, Mesh mesh)
        {
            VoxelData = voxelData;
            Mesh = mesh;
        }

        public List<CoordData> VoxelData { get; private set; }
        public Mesh Mesh { get; private set; }
    }
}