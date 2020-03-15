using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class LightController : MonoBehaviour
    {
        public LightDir lightDir;

        private void Update()
        {
            transform.position = lightDir.GetDirVector() * -1;
            transform.LookAt(Vector3.zero, Vector3.up);
        }
    }
}