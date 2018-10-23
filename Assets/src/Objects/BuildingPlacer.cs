using UnityEngine;

namespace FarmVox
{
    static class BuildingPlacer
    {
        public static void Place(Building building, Chunks chunks, HeightMap.Tile tile)
        {
            if (!tile.Center.HasValue)
            {
                return;
            }

            var position = tile.Center.Value;
            var model = VoxLoader.Load(building.ModelName);
            foreach (var voxel in model.Voxels) {
                var color = model.Palette[voxel.ColorIndex - 1];
                var coord = new Vector3Int(voxel.X, voxel.Y, voxel.Z) + building.Offset;

                chunks.Set(coord + position, 1.0f);
                chunks.SetColor(coord + position, color);
            }
        }
    }
}