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
        private readonly QuadTree<GameObject> _structures = new QuadTree<GameObject>(32);

        public bool CanPlaceBuilding(BuildingTile buildingTile, StructureType structureType)
        {
//            var buildingCoord = buildingTile.buildingCoord;
//            var modelObject = LoadObject(structureType, buildingTile);
//            var size = modelObject.GetSize();
//
//            var hasExistingBuilding = _structures.Search(new BoundsInt(buildingCoord, size)).Any();
//
//            return !hasExistingBuilding &&
//                   buildingTile.isBuildable;
            return true;
        }

        public bool TryPlaceBuilding(IEnumerable<BuildingTile> tiles, StructureData structureData)
        {
            var tileList = tiles.ToArray();

            if (tileList.Any(t => t.hasBuilding))
            {
                return false;
            }

            var buildingCoord = tileList
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

            _structures.Add(buildingCoord, structureGo);

            foreach (var tile in tileList)
            {
                tile.hasBuilding = true;
            }

            if (structureData.type == StructureType.Wall)
            {
                var wall = structureGo.AddComponent<Wall>();
                var tile = tileList.First();
                var origin = tile.GetBuildingCoord();
                wall.origin = origin;
                wall.size = new Vector2Int(3, 3);
                wall.height = 24;
                wall.terrian = FindObjectOfType<Terrian>();
                wall.chunks = chunks;
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

    public class Wall : MonoBehaviour
    {
        public Vector3Int origin;
        public Chunks chunks;
        public Vector2Int size;
        public int height;
        public Terrian terrian;
        public Color wallColor = new Color(0.4f, 0.4f, 0.4f);

        private void Start()
        {
            for (var i = 0; i < size.x; i++)
            {
                for (var k = 0; k < size.y; k++)
                {
                    for (var j = origin.y; j < height; j++)
                    {
                        var coord = new Vector3Int(origin.x + i, j, origin.z + k);
                        var isGround = terrian.IsGround(coord);

                        if (!isGround)
                        {
                            chunks.Set(coord, 1.0f);
                            chunks.SetColor(coord, wallColor);
                        }
                    }
                }
            }
        }
    }
}