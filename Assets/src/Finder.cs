using UnityEngine;

namespace FarmVox
{
    public static class Finder
    {
        public static CameraController FindCameraController() {
            return Camera.main.GetComponent<CameraController>();
        }
    }
}