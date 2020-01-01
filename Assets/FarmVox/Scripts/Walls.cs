using System;
using System.Linq;
using FarmVox.Models;
using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(Chunks))]
    public class Walls : MonoBehaviour
    {
        public Chunks chunks;
        public GameObject structurePrefab;
        public GameObject wallPrefab;
        public Terrian terrian;

        public TextAsset sawmill;
        public TextAsset house;
        public TextAsset wall;

        public int wallHeight = 6;
        

        private readonly QuadTree<GameObject> _structures = new QuadTree<GameObject>(32);

        public void PlaceBuilding(BuildingTile tile, StructureType structureType)
        {
//            var buildingCoord = tile.buildingCoord;
//            var modelObject = LoadObject(structureType, tile);
//            var size = modelObject.GetSize();
//
//            var structureGo = Instantiate(GetStructurePrefab(structureType), transform);
//            structureGo.transform.position = buildingCoord + new Vector3(size.x / 2f, 0, size.z / 2.0f);
//            var volume = structureGo.GetComponent<NavMeshModifierVolume>();
//            volume.size = size;
//            volume.center = new Vector3(0, size.y / 2.0f, 0);
//
//            ObjectPlacer.Place(chunks, modelObject, buildingCoord, Time.frameCount % 4);
//
//            _structures.Add(buildingCoord, structureGo);
        }

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
            return false;
        }

        private IPlaceableObject LoadObject(StructureType structureType, BuildingTile tile)
        {
//            if (structureType == StructureType.Wall)
//            {
//                var height = wallHeight + terrian.Config.GroundHeight - tile.buildingCoord.y;
//                return new WallObject(height);
//            }
//
//            var asset = GetAsset(structureType);
//            var model = ModelLoader.Load(asset);
//            return new ModelObject(model);
            return null;
        }

        private GameObject GetStructurePrefab(StructureType structureType)
        {
            return structureType == StructureType.Wall ? wallPrefab : structurePrefab;
        }

        private TextAsset GetAsset(StructureType structureType)
        {
            switch (structureType)
            {
                case StructureType.House:
                    return house;
                case StructureType.Sawmill:
                    return sawmill;
                default:
                    throw new ArgumentOutOfRangeException(nameof(structureType), structureType, null);
            }
        }
    }

    public enum StructureType
    {
        House,
        Sawmill,
        Wall
    }
}