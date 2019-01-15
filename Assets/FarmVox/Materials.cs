using UnityEngine;

namespace FarmVox
{

    public static class Materials
    {
        public static Material GetVoxelMaterial() {
            return new Material(Shader.Find("Unlit/voxelunlit"));
        }

        public static Material GetVoxelMaterialTrans() {
            return new Material(Shader.Find("Unlit/voxeltrans"));
        }
        
        public static Material GetVoxelMaterialModel() {
            return new Material(Shader.Find("Unlit/voxelmodel"));
        }
    }
}