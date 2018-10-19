using FarmVox;
using UnityEngine;

public class Commander : MonoBehaviour
{
    HighlightHoveredSurface highlight;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            var result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (result != null) {
                var house = new House();
                house.Add(Finder.FindTerrian().BuildingLayer, result.GetCoord());
            }
        }
    }
}
