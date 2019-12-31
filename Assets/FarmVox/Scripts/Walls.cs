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

        public Vector3Int GetBuildingGrid(Vector3Int coord)
        {
            return
                new Vector3Int(
                    Mathf.FloorToInt(coord.x / 6.0f) * 6,
                    coord.y,
                    Mathf.FloorToInt(coord.z / 6.0f) * 6);
        }

        public void PlaceBuilding(TextAsset asset, Vector3Int coord)
        {
            var model = ModelLoader.Load(asset);
            var modelObject = new ModelObject(model);

            var structureGo = Instantiate(structurePrefab, transform);
            var size = modelObject.GetSize();
            structureGo.transform.position = coord + new Vector3(size.x / 2f, 0, size.z / 2.0f);
            var volume = structureGo.GetComponent<NavMeshModifierVolume>();
            volume.size = size;
            volume.center = new Vector3(0, size.y / 2.0f, 0);

            ObjectPlacer.Place(chunks, modelObject, coord, Time.frameCount % 4);
        }
    }
}