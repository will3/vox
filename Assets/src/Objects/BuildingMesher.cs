using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace FarmVox
{
    public class BuildingMesher
    {   
        private readonly Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

        public Mesh Mesh(Building building)
        {
            var key = building.Name + building.Variation;
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
            
            var modelSize = model.Size;
            var size = Mathf.Max(modelSize.x, modelSize.y, modelSize.z);
            using (var mesher = new MesherGpu(size))
            {
                var data = new float[size * size * size];
                var colors = new Color[size * size * size];

                foreach (var voxel in model.Voxels)
                {
                    var index = voxel.X * size * size + voxel.Y * size + voxel.Z;
                    data[index] = 1.0f;
                    colors[index] = model.Palette[voxel.ColorIndex];
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