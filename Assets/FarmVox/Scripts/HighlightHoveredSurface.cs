using UnityEngine;
using System.Collections;
using FarmVox;
using FarmVox.Voxel;
using UnityEngine.Serialization;
using Terrian = FarmVox.Terrain.Terrian;

namespace FarmVox.Scripts
{
    public class HighlightHoveredSurface : MonoBehaviour
    {
        public Terrian Terrian;

        public VoxelRaycastResult result;

        private GameObject go;

        // Use this for initialization
        void Start()
        {
            go = new GameObject("highlight");
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

            result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (result != null)
            {
                var mesh = result.GetQuad();
                var pos = result.GetCoord();
                go.GetComponent<MeshFilter>().mesh = mesh;
                // go.transform.position = pos + result.hit.normal * 0.1f;
            }
        }
    }
}