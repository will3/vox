using System;
using FarmVox.Models;
using FarmVox.Objects;
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

        public TextAsset sawmill;
        public TextAsset house;
        public TextAsset wall;

        public Vector3Int GetBuildingGrid(Vector3Int coord)
        {
            return
                new Vector3Int(
                    Mathf.FloorToInt(coord.x / 6.0f) * 6,
                    coord.y,
                    Mathf.FloorToInt(coord.z / 6.0f) * 6);
        }

        public void PlaceBuilding(StructureType structureType, Vector3Int coord)
        {
            var modelObject = LoadObject(structureType);
            var structureGo = Instantiate(structurePrefab, transform);
            var size = modelObject.GetSize();
            structureGo.transform.position = coord + new Vector3(size.x / 2f, 0, size.z / 2.0f);
            var volume = structureGo.GetComponent<NavMeshModifierVolume>();
            volume.size = size;
            volume.center = new Vector3(0, size.y / 2.0f, 0);

            ObjectPlacer.Place(chunks, modelObject, coord, Time.frameCount % 4);
        }

        private IPlaceableObject LoadObject(StructureType structureType)
        {
            if (structureType == StructureType.Wall)
            {
                return new Wall();
            }
            var asset = GetAsset(structureType);
            var model = ModelLoader.Load(asset);
            return new ModelObject(model);
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