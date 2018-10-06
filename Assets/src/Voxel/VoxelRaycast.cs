using UnityEngine;

namespace FarmVox
{
    class VoxelRaycast
    {
        public static RaycastResult TraceMouse(int layerMask = 0)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return TraceRay(ray, layerMask);
        }

        public static RaycastResult TraceRay(Ray ray, int layerMask = 0) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                return new RaycastResult(hit);
            }

            return null;
        }
    }
}