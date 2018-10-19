using UnityEngine;

namespace FarmVox
{
    public static class Finder
    {
        private static GameController FindGameController() {
            var go = GameObject.FindWithTag("GameController");
            return go == null ? null : go.GetComponent<GameController>();
        }

        public static Terrian FindTerrian() {
            var gc = FindGameController();
            return gc == null ? null : gc.Terrian;
        }
    }
}