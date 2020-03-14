using UnityEngine;

namespace FarmVox.Scripts
{
    public class LightController : MonoBehaviour
    {
        public Vector3Int lightDir = new Vector3Int(-1, -1, -1);
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Logger.LogComponentNotFound(typeof(Camera));
            }
            transform.position = lightDir * -1;
            transform.LookAt(Vector3.zero, Vector3.up);
        }

        // TODO
        private void Update()
        {
            /*var forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);
            var right = Vector3.Cross(Vector3.up, forward).normalized * Mathf.Pow(2, 0.5f);
            var dir = new Vector3(right.x, 1, right.z) * -1;*/
        }
    }
}