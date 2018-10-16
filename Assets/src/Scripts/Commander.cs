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
            var result = VoxelRaycast.TraceMouse(1 << UserLayers.terrian);
            if (result != null) {
                var model = new VoxelModel("house");
                model.Add(Finder.FindTerrian().BuildingLayer, result.GetCoord());
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Finder.FindTerrian().UpdateReflection();
        }
    }
}
