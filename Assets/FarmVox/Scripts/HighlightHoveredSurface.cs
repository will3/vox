using UnityEngine;
using FarmVox.Voxel;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HighlightHoveredSurface : MonoBehaviour
    {
        public VoxelRaycastResult result;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        
        private Vector3Int _lastCoord;
        private Mesh _mesh;

        private void Update()
        {
            result = VoxelRaycast.TraceMouse(1 << UserLayers.Terrian);
            if (result == null)
            {
                return;
            }

            var coord = result.GetCoord();
            if (_lastCoord == coord)
            {
                return;
            }

            if (_mesh != null)
            {
                Destroy(_mesh);
            }

            _mesh = result.GetQuad();
            meshFilter.mesh = _mesh;
            transform.position = coord + result.Hit.normal * 0.1f;
            _lastCoord = coord;
        }
    }
}