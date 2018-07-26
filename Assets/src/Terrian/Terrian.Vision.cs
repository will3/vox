using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateVision() {
            var visionMap = Finder.FindGameController().VisionMap;
            material.SetBuffer("_VisionBuffer", visionMap.GetComputeBuffer());

            var origin = new Vector2(visionMap.Origin.x, visionMap.Origin.y);
            material.SetVector("_VisionOrigin", origin);
            material.SetInt("_VisionSize", visionMap.Size);
            material.SetInt("_VisionResolution", visionMap.Resolution);
        }
    }
}
