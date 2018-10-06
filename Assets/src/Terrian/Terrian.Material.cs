using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void UpdateMaterial() {
            foreach (var chunks in chunksToDraw) {
                foreach (var chunk in chunks.Map.Values) {
                    var material = chunk.Material;

                    var origin = chunk.Origin;
                    material.SetVector("_Origin", (Vector3)origin);
                    material.SetInt("_Size", size);

                    shadowMap.UpdateMaterial(material, origin);
                }
            }
        }
    }
}
