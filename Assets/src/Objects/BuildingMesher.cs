using UnityEngine;

namespace FarmVox
{
    public class BuildingMesher
    {        
        public Mesh Mesh(Building building)
        {
            var modelSize = building.Model.Size;
            var size = Mathf.Max(modelSize.x, modelSize.y, modelSize.z);
            var mesher = new MesherGpu(size);

            var data = new float[size * size * size];
            var colors = new Color[size * size * size];

            foreach (var voxel in building.Model.Voxels)
            {
                var index = voxel.X * size * size + voxel.Y * size + voxel.Z;
                data[index] = 1.0f;
                colors[index] = building.Model.Palette[voxel.ColorIndex];
            }
            
            var voxelBuffer = mesher.CreateVoxelBuffer();
            voxelBuffer.SetData(data);
            
            var colorBuffer = mesher.CreateColorBuffer();
            colorBuffer.SetData(colors);
            
            var triangleBuffer = mesher.CreateTrianglesBuffer();

            foreach (var voxel in building.Model.Voxels)
            {
                
            }
            
            // mesher.Dispatch();

            return null;
        }
    }
}