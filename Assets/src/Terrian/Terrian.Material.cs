using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateMaterial() {
            var visionMap = Finder.FindGameController().VisionMap;
            visionMap.UpdateBuffer();
            float shadowStrength = 1.0f;

            foreach (var tc in map.Values) {
                var material = tc.Material;
                material.SetBuffer("_VisionBuffer", visionMap.VisionBuffer);
                material.SetInt("_MaxVisionNumber", VisionMap.MaxVisionNumber);
                material.SetInt("_UseVision", 0);
                var origin = tc.Origin;
                material.SetVector("_Origin", (Vector3)origin);
                material.SetInt("_Size", size);

                material.SetBuffer("_ShadowMap00", shadowMap.GetBuffer(origin, new Vector2Int(0, 0)));
                material.SetBuffer("_ShadowMap01", shadowMap.GetBuffer(origin, new Vector2Int(0, 1)));
                material.SetBuffer("_ShadowMap10", shadowMap.GetBuffer(origin, new Vector2Int(1, 0)));
                material.SetBuffer("_ShadowMap11", shadowMap.GetBuffer(origin, new Vector2Int(1, 1)));

                material.SetFloat("_ShadowStrength", shadowStrength);
            }
        }
    }
}
