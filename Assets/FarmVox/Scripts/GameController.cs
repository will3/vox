using UnityEngine;

namespace FarmVox.Scripts
{
    public class GameController : MonoBehaviour
    {
        public int seed = 1337;

        private void Awake()
        {
            NoiseUtils.SetSeed(1337);
            Application.targetFrameRate = 60;
        }
    }
}