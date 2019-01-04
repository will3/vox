using UnityEngine;
using FarmVox.Voxel;

namespace FarmVox.Scripts
{
    public class HighlightHoveredSurface : MonoBehaviour
    {
        public VoxelRaycastResult Result;

        private GameObject _go;

        // Use this for initialization
        private void Start()
        {
            _go = new GameObject("highlight");
            _go.AddComponent<MeshFilter>();
            _go.AddComponent<MeshRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            Result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (Result == null) return;
            var mesh = Result.GetQuad();
            var pos = Result.GetCoord();
            _go.GetComponent<MeshFilter>().mesh = mesh;
            _go.transform.position = pos + Result.Hit.normal * 0.1f;
        }
    }
}