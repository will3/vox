using UnityEngine;

namespace FarmVox.Scripts
{
    public class GameController : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}