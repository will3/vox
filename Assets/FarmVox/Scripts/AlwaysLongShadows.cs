using System.Collections.Generic;
using System.Linq;
using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class AlwaysLongShadows : MonoBehaviour
    {
        private Camera _camera;
        private LightController _lightController;

        private Dictionary<Vector3, LightDir> _dirs = new Dictionary<Vector3, LightDir>
        {
            {new Vector3(-1, 0, -1).normalized, LightDir.NorthEast},
            {new Vector3(-1, 0, 1).normalized, LightDir.SouthEast},
            {new Vector3(1, 0, -1).normalized, LightDir.NorthWest},
            {new Vector3(1, 0, 1).normalized, LightDir.SouthWest}
        };

        private void Start()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Logger.LogComponentNotFound(typeof(Camera));
            }

            _lightController = FindObjectOfType<LightController>();
            if (_lightController == null)
            {
                Logger.LogComponentNotFound(typeof(LightController));
            }
        }

        private void Update()
        {
            var forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var right = Vector3.Cross(forward, Vector3.up);

            var maxDot = 0f;
            var maxDir = LightDir.NorthEast;
            foreach (var kv in _dirs)
            {
                var dot = Vector3.Dot(right, kv.Key);
                if (!(dot > maxDot))
                {
                    continue;
                }
                maxDot = dot;
                maxDir = kv.Value;
            }

            _lightController.lightDir = maxDir;
        }
    }
}