using System.Collections.Generic;
using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Objects
{
    public class BuildingMesher
    {   
        private readonly Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

        public Mesh Mesh(Building building)
        {
            var key = building.ModelName;
            if (_meshes.ContainsKey(key))
            {
                return _meshes[key];
            }

            _meshes[key] = _Mesh(building);
            return _meshes[key];
        }
        
        private Mesh _Mesh(Building building)
        {
            var model = VoxLoader.Load(building.Name);
            
            // var modelSize = model.Size;
            // var size = Mathf.Max(modelSize.x, modelSize.y, modelSize.z);
            var size = 32;
            using (var mesher = new MesherGpu(size, Terrian.Instance.Config))
            {
                var data = new float[size * size * size];
                var colors = new Color[size * size * size];

                foreach (var voxel in model.Voxels)
                {
                    var index = (voxel.X + 1) * size * size + (voxel.Y + 1) * size + (voxel.Z + 1);
                    data[index] = 1.0f;
                    colors[index] = model.Palette[voxel.ColorIndex - 1];
                }
                
                mesher.SetData(data);
                mesher.SetColors(colors);
            
                mesher.Dispatch();

                var triangles = mesher.ReadTriangles();

                var mesh = new MeshBuilder().AddTriangles(triangles).Build();

                return mesh;   
            }
        }
    }
}