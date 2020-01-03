using System;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Models;
using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class StructureData
    {
        public StructureType type;
        public Vector2Int numGrids;
        public string modelName;
    }

    [Serializable]
    public class StructureDataRoot
    {
        public StructureData[] structures;
    }

    [RequireComponent(typeof(Chunks))]
    public class Structures : MonoBehaviour
    {
        public BuildingTiles tiles;
        public Chunks chunks;
        public GameObject structurePrefab;
        public int wallHeightMultiplier = 5;
        public Terrian terrian;
        public Water water;
        public Ground ground;

        private readonly QuadTree<GameObject> _structures = new QuadTree<GameObject>(32);

        public bool TryPlaceBuilding(IEnumerable<BuildingTile> tileList, StructureData structureData)
        {
            var tl = tileList.ToArray();
            if (tl.Any(t => t.hasBuilding))
            {
                return false;
            }

            var buildingCoord = tl
                .OrderBy(t => t.bounds.min.x)
                .ThenBy(t => t.bounds.min.z)
                .First()
                .GetBuildingCoord();

            if (structureData.modelName != null)
            {
                var modelObject = LoadObject(structureData);
                ObjectPlacer.Place(chunks, modelObject, buildingCoord, Time.frameCount % 4);
            }

            var grids = structureData.numGrids;

            var structureGo = Instantiate(structurePrefab, transform);
            structureGo.transform.position = buildingCoord + new Vector3(grids.x * this.tiles.gridSize / 2f, 0,
                                                 grids.y * this.tiles.gridSize / 2.0f);

            var structure = structureGo.GetComponent<Structure>();
            structure.chunks = chunks;
            structure.ground = ground;
            structure.origin = buildingCoord;
            structure.gridSize = tiles.gridSize;
            structure.numGrids = structureData.numGrids;
            structure.structureType = structureData.type;
            var y = Mathf.Max(buildingCoord.y, water.config.waterLevel); 
            structure.wallHeight = (Mathf.CeilToInt(y / (float) wallHeightMultiplier) + 3)
                                   * wallHeightMultiplier;
            _structures.Add(buildingCoord, structureGo);

            foreach (var tile in tl)
            {
                tile.hasBuilding = true;
            }

            return true;
        }

        private static IPlaceableObject LoadObject(StructureData structureData)
        {
            var path = $"models/{structureData.modelName}";
            var textAsset = Resources.Load<TextAsset>(path);
            var model = JsonUtility.FromJson<Model>(textAsset.text);
            return new ModelObject(model);
        }
    }

    public enum StructureType
    {
        House = 1,
        Wall = 2
    }
}