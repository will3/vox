using UnityEngine;

namespace FarmVox.Scripts
{
    public class CenterCameraAroundWorld : MonoBehaviour
    {
        public CameraController cameraController;
        public Ground ground;
        public float targetY = 16;

        private void Start()
        {
            if (cameraController == null)
            {
                cameraController = FindObjectOfType<CameraController>();
            }

            if (ground == null)
            {
                ground = FindObjectOfType<Ground>();
            }

            var target = ground.Center;
            target.y = targetY;
            cameraController.target = target;
        }
    }
}