using FarmVox.Scripts;
using UnityEngine;

namespace FarmVox
{
    public static class Finder
    {
        public static GameController FindGameController() {
            var go = GameObject.FindWithTag("GameController");
            return go == null ? null : go.GetComponent<GameController>();
        }

        public static Terrain.Terrian FindTerrian() {
            var gc = FindGameController();
            return gc == null ? null : gc.Terrian;
        }
    }
}