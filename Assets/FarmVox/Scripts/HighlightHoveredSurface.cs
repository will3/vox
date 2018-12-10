using UnityEngine;
using System.Collections;
using FarmVox;
using FarmVox.Voxel;
using Terrian = FarmVox.Terrain.Terrian;

namespace FarmVox.Scripts
{
    public class HighlightHoveredSurface : MonoBehaviour
    {
        private Terrian _terrian;

        private Terrian Terrian
        {
            get
            {
                if (_terrian == null)
                {
                    _terrian = GameObject.FindWithTag("GameController").GetComponent<GameController>().Terrian;
                }

                return _terrian;
            }
        }

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