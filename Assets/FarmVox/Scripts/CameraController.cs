using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 target = new Vector3(0, 32, 0);
        public Vector3 rotation = new Vector3(45, 45, 0);
        public Vector3 targetRotation = new Vector3(45, 45, 0);
        public float distance = 300;
        public bool keysEnabled = true;
        public float orthographicSize = 50;
        public float rotateAmount = 90f;
        public float cameraRotateSpeed = 0.2f;
        public float zoom = 1.0f;
        public float zoomSpeed = 1.1f;

        private float _forward;
        private float _right;
        private Vector3 _lastRightDragPosition;
        private bool _rightDragging;
        private Vector3 _lastLeftDragPosition;
        private bool _leftDragging;

        private float _rotate;

        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                Logger.LogComponentNotFound(typeof(Camera));
            }
        }

        private void UpdateKeys()
        {
            var rotate = 0.0f;
            if (Input.GetKeyUp(KeyCode.Q)) rotate += 1.0f;
            if (Input.GetKeyUp(KeyCode.E)) rotate -= 1.0f;

            if (Input.GetKeyUp(KeyCode.Equals)) zoom /= zoomSpeed;
            if (Input.GetKeyUp(KeyCode.Minus)) zoom *= zoomSpeed;

            targetRotation.y += rotate * rotateAmount;
        }

        private void Update()
        {
            if (keysEnabled)
            {
                UpdateKeys();
            }

            rotation = Vector3.Lerp(rotation, targetRotation, cameraRotateSpeed);
            var dir = Quaternion.Euler(rotation) * Vector3.forward;
            var position = target - dir * distance;
            transform.position = position;
            transform.LookAt(target, Vector3.up);

            if (_camera.orthographic)
            {
                _camera.orthographicSize = orthographicSize * zoom;
            }
        }
    }
}