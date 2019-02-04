using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Cleanup : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            Chunk.Cleanup();
        }
    }
}