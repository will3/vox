using UnityEngine;

namespace FarmVox.Voxel
{
    public interface IWaterfallChunk
    {
        void SetWaterfall(Vector3Int coord, float v);
        float GetWaterfall(Vector3Int coord);
    }
}