using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateMaterial() {
            var visionMap = Finder.FindGameController().VisionMap;
            visionMap.UpdateBuffer();

            foreach (var chunks in chunksToDraw) {
                foreach (var chunk in chunks.Map.Values) {
                    var material = chunk.Material;
                    material.SetBuffer("_VisionBuffer", visionMap.VisionBuffer);
                    material.SetInt("_MaxVisionNumber", VisionMap.MaxVisionNumber);
                    material.SetInt("_UseVision", 0);
                    var origin = chunk.Origin;
                    material.SetVector("_Origin", (Vector3)origin);
                    material.SetInt("_Size", size);

                    material.SetBuffer("_ShadowMap00", shadowMap.GetBuffer(origin, new Vector2Int(0, 0)));
                    material.SetBuffer("_ShadowMap01", shadowMap.GetBuffer(origin, new Vector2Int(0, 1)));
                    material.SetBuffer("_ShadowMap10", shadowMap.GetBuffer(origin, new Vector2Int(1, 0)));
                    material.SetBuffer("_ShadowMap11", shadowMap.GetBuffer(origin, new Vector2Int(1, 1)));
                    material.SetInt("_ShadowMapSize", shadowMap.DataSize);
                    material.SetFloat("_ShadowStrength", TerrianConfig.Instance.shadowStrength);

                    material.SetBuffer("_BuildGrid", buildGrid.GetBuffer(origin));

                    if (Commander.instance != null) {
                        material.SetInt("_ShowGrid", Commander.instance.isBuilding ? 1 : 0);
                    }
                }
            }
        }
    }
}
