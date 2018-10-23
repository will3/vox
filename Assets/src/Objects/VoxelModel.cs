using System.Collections.Generic;
using Kaitai;
using UnityEngine;
using Application = UnityEngine.Application;

namespace FarmVox
{
    public class VoxelModel
    {
        public Vector3Int Offset { get; private set; }
        public readonly List<Vox.Voxel> Voxels = new List<Vox.Voxel>();
        public readonly List<Color> Palette = new List<Color> {
            Colors.GetColor("#654f30"),
            Colors.GetColor("#705836"),
            Colors.GetColor("#ac8956"),
            Colors.GetColor("#4d4232"),
            Colors.GetColor("#676767")
        };
        
        public VoxelModel(string name, Vector3Int offset) {
            Offset = offset;
            var data = Vox.FromFile(Application.dataPath + "/Resources/vox/" + name + ".vox");
            VoxLoader.LoadData(data, Voxels);
        }
    }
}