using UnityEngine;

namespace FarmVox
{

    public class Materials
    {
        public static Material GetVoxelMaterial() {
            return new Material(Shader.Find("Unlit/voxelunlit"));
        }

        public static Material GetVoxelMaterialTrans() {
            return new Material(Shader.Find("Unlit/voxeltrans"));
        }
    }
}