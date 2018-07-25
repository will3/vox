using UnityEngine;

namespace FarmVox
{
    public static class Finder
    {
        public static CameraController FindCameraController() {
            return Camera.main.GetComponent<CameraController>();
        }

        public static GameController FindGameController() {
            return GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }

        public static Terrian FindTerrian() {
            return GameObject.FindWithTag("GameController").GetComponent<GameController>().Terrian;
        }
    }
}