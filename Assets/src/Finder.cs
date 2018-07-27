using UnityEngine;

namespace FarmVox
{
    public static class Finder
    {
        public static CameraController FindCameraController() {
            return Camera.main.GetComponent<CameraController>();
        }

        public static GameController FindGameController() {
            var go = GameObject.FindWithTag("GameController");
            if (go == null) { return null; }
            return go.GetComponent<GameController>();
        }

        public static Terrian FindTerrian() {
            var gc = FindGameController();
            if (gc == null) { return null; }
            return gc.Terrian;
        }
    }
}