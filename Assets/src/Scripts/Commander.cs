using FarmVox;
using UnityEngine;

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
                var house = new Building("house");
                
            }
        }
    }
}
