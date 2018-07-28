using UnityEngine;

namespace FarmVox
{
    class VoxelRaycast
    {
        public static RaycastResult TraceMouse()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return new RaycastResult(hit);
            }

            return null;
        }
    }
}