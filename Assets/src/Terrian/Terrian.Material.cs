using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateMaterial() {
            var visionMap = Finder.FindGameController().VisionMap;
            visionMap.UpdateBuffer();

            foreach (var tc in map.Values) {
                var material = tc.Material;
                material.SetBuffer("_VisionBuffer", visionMap.VisionBuffer);
                material.SetInt("_MaxVisionNumber", VisionMap.MaxVisionNumber);
                material.SetInt("_UseVision", 0);
                var origin = new Vector3(tc.Origin.x, tc.Origin.y, tc.Origin.z);
                material.SetVector("_Origin", origin);
            }
        }
    }
}
