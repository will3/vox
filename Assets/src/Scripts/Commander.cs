using FarmVox;
using UnityEngine;

public class Commander : MonoBehaviour
{
    HighlightHoveredSurface highlight;

    // Use this for initialization
    void Start()
    {
        highlight = gameObject.AddComponent<HighlightHoveredSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (highlight.result != null) {
                var model = new VoxelModel("house");
                model.Add(Finder.FindTerrian().BuildingLayer, highlight.result.GetCoord());
            }
        }
    }
}
