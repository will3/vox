using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class BuildWallCommand : Command
    {
        GameObject highlightObject;

        Vector3Int buildCoord;

        HashSet<GameObject> cubes = new HashSet<GameObject>();

        public override bool Update()
        {
            var layerMask = 1 << UserLayers.terrian;
            var result = VoxelRaycast.TraceMouse(layerMask);

            if (highlightObject == null)
            {
                highlightObject = new GameObject("highlight");
            }

            if (result != null) {
                var coord = result.GetCoord();
                var bCoord = new Vector3Int(
                    Mathf.FloorToInt(coord.x / 3.0f) * 3,
                    Mathf.FloorToInt(coord.y / 3.0f) * 3,
                    Mathf.FloorToInt(coord.z / 3.0f) * 3
                );

                if (bCoord != this.buildCoord) {
                    foreach(var c in cubes) {
                        Object.Destroy(c);
                    }

                    var surfaceCoord = FindSurfaceCoord(buildCoord);

                    if (surfaceCoord != null) {
                        var cube = new GameObject("cube");
                        cube.transform.parent = highlightObject.transform;
                        var meshFilter = cube.AddComponent<MeshFilter>();
                        var meshRenderer = cube.AddComponent<MeshRenderer>();
                        meshFilter.sharedMesh = Cube.CubeMesh;
                        cube.transform.position = buildCoord;
                        var padding = new Vector3(0.1f, 0.1f, 0.1f);
                        cube.transform.position -= padding;
                        cube.transform.localScale += padding;
                        cubes.Add(cube);
                    }


                    this.buildCoord = bCoord;
                }
            }

            return false;
        }

        public Vector3Int? FindSurfaceCoord(Vector3Int from) {
            int maxTry = 100;
            var defaultLayer = Finder.FindTerrian().DefaultLayer;
            for (var i = 0; i < maxTry; i++) {
                var coord = from + new Vector3Int(0, i + 1, 0);
                var v = defaultLayer.Get(coord);
                if (v <= 0) {
                    return from + new Vector3Int(0, i, 0);
                }
            }
            return null;
        }
    }
}