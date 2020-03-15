using UnityEngine;

namespace FarmVox.Scripts
{
    public class CenterCameraAroundWorld : MonoBehaviour
    {
        public CameraController cameraController;
        public World world;
        public float targetY = 16;

        private void Start()
        {
            if (cameraController == null)
            {
                cameraController = FindObjectOfType<CameraController>();
            }

            if (world == null)
            {
                world = FindObjectOfType<World>();
            }

            var target = world.Center;
            target.y = targetY;
            cameraController.target = target;
        }
    }
}