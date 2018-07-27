using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateVision() {
            var visionMap = Finder.FindGameController().VisionMap;
            visionMap.UpdateBuffer();
            material.SetBuffer("_VisionBuffer", visionMap.VisionBuffer);
            material.SetInt("_MaxVisionNumber", VisionMap.MaxVisionNumber);
            material.SetInt("_UseVision", 1);
        }
    }
}
