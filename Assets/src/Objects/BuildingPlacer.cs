using UnityEngine;

namespace FarmVox
{
    static class BuildingPlacer
    {
        public static void Add(VoxelModel model, Chunks chunks, HeightMap.Tile tile)
        {
            Add(model, chunks, tile.Center);
        }
        
        public static void Add(VoxelModel model, Chunks chunks, Vector3Int position) {
            foreach (var voxel in model.Voxels) {
                var color = model.Palette[voxel.ColorIndex - 1];
                var coord = new Vector3Int(voxel.X, voxel.Z, voxel.Y) + model.Offset;

                chunks.Set(coord + position, 1.0f);
                chunks.SetColor(coord + position, color);
            }
        }
    }
}