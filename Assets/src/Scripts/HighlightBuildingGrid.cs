using UnityEngine;
using System.Collections;

namespace FarmVox
{
    public class HighlightBuildingGrid : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (result != null) {
                var coord = result.GetCoord();

                var tile = Finder.FindTerrian().heightMap.GetTile(coord);
                if (tile == null || !tile.CanBuild()) {
                    // Hide object
                } else {
                    // Show object
                    var go = new GameObject();
                    var meshFilter = go.AddComponent<MeshFilter>();
                    var meshRenderer = go.AddComponent<MeshRenderer>();
                    
                    var meshBuilder = new CubeMeshBuilder();
                    foreach (var tileCoord in tile.Coords.Values)
                    {
                        meshBuilder.AddQuad(Axis.Y, true, tileCoord);
                    }

                    meshFilter.mesh = meshBuilder.Build();
                    
                    // go.transform.position = coord + result.hit.normal * 0.1f;
                }
            }
        }
    }
}