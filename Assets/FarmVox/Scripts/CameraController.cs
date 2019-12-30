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

        private float _forward;
        private float _right;
        private Vector3 _lastRightDragPosition;
        private bool _rightDragging;
        private Vector3 _lastLeftDragPosition;
        private bool _leftDragging;

        private float _rotate;

        private void UpdateKeys()
        {
            var rotate = 0.0f;
            if (Input.GetKeyUp(KeyCode.Q)) rotate += 1.0f;
            if (Input.GetKeyUp(KeyCode.E)) rotate -= 1.0f;

            targetRotation.y += rotate * 90f;
        }

        private void Update()
        {
            if (keysEnabled)
            {
                UpdateKeys();
            }

            rotation = Vector3.Lerp(rotation, targetRotation, 0.2f);
            var dir = Quaternion.Euler(rotation) * Vector3.forward;
            var position = target - dir * distance;
            transform.position = position;
            transform.LookAt(target, Vector3.up);

            var c = GetComponent<Camera>();
            if (c.orthographic)
            {
                c.orthographicSize = orthographicSize;
            }
        }
    }
}