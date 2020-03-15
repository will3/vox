using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class LightController : MonoBehaviour
    {
        public LightDir lightDir;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Logger.LogComponentNotFound(typeof(Camera));
            }
        }

        private void Update()
        {
            transform.position = lightDir.GetDirVector() * -1;
            transform.LookAt(Vector3.zero, Vector3.up);
        }
    }
}