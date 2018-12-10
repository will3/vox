using FarmVox.Objects;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class BuildingScript : MonoBehaviour
    {
        public Building Building;
        
        private void Start()
        {
            var model = VoxLoader.Load(Building.ModelName);

            if (Building.Tile.Center.HasValue)
            {
                BuildingPlacer.Place(Building, Terrian.Instance.BuildingLayer, Building.Tile);    
            }
            
//            var mesh = new BuildingMesher().Mesh(Building);
//            _meshFilter.mesh = mesh;
//
//            if (Building.Tile != null && Building.Tile.Center.HasValue)
//            {
//                transform.position = Building.Tile.Center.Value;
//            }
        }
    }
}