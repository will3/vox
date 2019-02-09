using UnityEngine;

namespace FarmVox.Voxel
{
    public static class VoxelRaycast
    {
        public static VoxelRaycastResult TraceMouse(int layerMask)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return TraceRay(ray, layerMask);
        }

        public static VoxelRaycastResult TraceMouse(Vector3 mousePosition, int layerMask)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            return TraceRay(ray, layerMask);
        }
        
        public static VoxelRaycastResult TraceRay(Ray ray, int layerMask) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                return new VoxelRaycastResult(hit);
            }

            return null;
        }
    }
}