using UnityEngine;

namespace FarmVox.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 Target = new Vector3(0, 32, 0);
        public float ZoomRate = 1.1f;
        public float MouseRotateSpeed = 0.2f;
        public float MouseMoveSpeed = 1.0f;

        public Vector3 Rotation = new Vector3(45, 45, 0);
        public float Distance = 300;

        public bool InputEnabled = true;

        private Vector3 _lastRightDragPosition;
        private bool _rightDragging;
        
        private Vector3 _lastLeftDragPosition;
        private bool _leftDragging;
        
        public float OrthographicSize = 100; 

        public Vector3 GetVector()
        {
            return (Target - transform.position).normalized;
        }

        private float _rotate;
        
        private void UpdateInput()
        {
            ProcessRightDrag();
            ProcessLeftDrag();

            var scroll = Input.GetAxis("Mouse ScrollWheel");

            var c = GetComponent<Camera>();
            if (c.orthographic)
            {
                if (scroll > 0)
                {
                    OrthographicSize *= ZoomRate;    
                }
                else if (scroll < 0)
                {
                    OrthographicSize /= ZoomRate;
                }
            }
            else
            {
                if (scroll > 0)
                {
                    Distance *= ZoomRate;    
                }
                else if (scroll < 0)
                {
                    Distance /= ZoomRate;
                }
            }
        }

        private void ProcessRightDrag()
        {
            if (!_rightDragging && Input.GetKey(KeyCode.Mouse1))
            {
                _rightDragging = true;
                _lastRightDragPosition = Input.mousePosition;
            }

            if (_rightDragging && !Input.GetKey(KeyCode.Mouse1))
            {
                _rightDragging = false;
            }

            if (_rightDragging)
            {
                var diff = Input.mousePosition - _lastRightDragPosition;
                Rotation.y += diff.x * MouseRotateSpeed;
                _lastRightDragPosition = Input.mousePosition;
            }
        }
        
        private void ProcessLeftDrag()
        {
            if (!_leftDragging && Input.GetKey(KeyCode.Mouse0))
            {
                _leftDragging = true;
                _lastLeftDragPosition = Input.mousePosition;
            }

            if (_leftDragging && !Input.GetKey(KeyCode.Mouse0))
            {
                _leftDragging = false;
            }

            if (_leftDragging)
            {
                var forward = (transform.rotation * Vector3.forward).normalized;
                var up = Vector3.up;
                var right = Vector3.Cross(forward, up);
                var top = Vector3.Cross(right, up);
                
                var diff = Input.mousePosition - _lastLeftDragPosition;
                Target += diff.x * right * MouseMoveSpeed;
                Target += diff.y * top * MouseMoveSpeed;
                _lastLeftDragPosition = Input.mousePosition;
            }
        }
        
        private void Update()
        {
            if (InputEnabled)
            {
                UpdateInput();    
            }

            var position = Target - Quaternion.Euler(Rotation) * Vector3.forward * Distance;
            transform.position = position;
            transform.LookAt(Target, Vector3.up);

            var c = GetComponent<Camera>();
            if (c.orthographic)
            {
                c.orthographicSize = Screen.height / 800f * OrthographicSize;
            }
        }
    }
}