using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Actor : MonoBehaviour
    {
        private Terrian _terrian;
        
        private void Start()
        {
            _terrian = Terrian.Instance;
        }

        private void Update()
        {
            
        }
    }
}