using UnityEngine;

namespace FarmVox.Voxel
{
    public class VoxelRaycast
    {
        public static VoxelRaycastResult TraceMouse(int layerMask = 0)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return TraceRay(ray, layerMask);
        }

        public static VoxelRaycastResult TraceRay(Ray ray, int layerMask = 0) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                return new VoxelRaycastResult(hit);
            }

            return null;
        }
    }
}