using FarmVox;
using FarmVox.Objects;
using FarmVox.Scripts;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Commander : MonoBehaviour
    {
        HighlightHoveredSurface highlight;

        HighlightBuildingGrid highlightBuildingGrid;

        // Use this for initialization
        void Start()
        {
            var go = new GameObject();
            highlightBuildingGrid = go.AddComponent<HighlightBuildingGrid>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var tile = highlightBuildingGrid.HoveredTile;
                if (tile.CanBuild)
                {
                    var go = new GameObject();

//                var building = BuildingFactory.GetHouse();
                    var building = BuildingFactory.GetWall();
                    building.Tile = tile;

                    var houseScript = go.AddComponent<BuildingScript>();
                    houseScript.Building = building;
                }
            }
        }
    }
}