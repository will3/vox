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
        public bool KeysEnabled = true;
        public float OrthographicSize = 100;
        public float Speed = 20f;
        public float RotateSpeed = 80f;
        public Vector3 Velocity;
        public float Friction = 0.001f;
        public float RotateX = 45;
        
        private float _forward;
        private float _right;
        private Vector3 _lastRightDragPosition;
        private bool _rightDragging;
        private Vector3 _lastLeftDragPosition;
        private bool _leftDragging;

        public Vector3 GetVector()
        {
            return (Target - transform.position).normalized;
        }

        private float _rotate;
        
        private void UpdateInput()
        {
            UpdateRightDrag();
            UpdateLeftDrag();
            
            if (KeysEnabled)
            {
                UpdateKeys();    
            }
            
            UpdateScroll();
        }

        private void UpdateRightDrag()
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
        
        private void UpdateLeftDrag()
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

        private void UpdateKeys()
        {
            _forward = 0.0f;
            if (Input.GetKey(KeyCode.W)) _forward += 1.0f;
            if (Input.GetKey(KeyCode.S)) _forward -= 1.0f;
            _right = 0.0f;
            if (Input.GetKey(KeyCode.A)) _right -= 1.0f;
            if (Input.GetKey(KeyCode.D)) _right += 1.0f;
            var rotate = 0.0f;
            if (Input.GetKey(KeyCode.Q)) rotate -= 1.0f;
            if (Input.GetKey(KeyCode.E)) rotate += 1.0f;
            Rotation.x = RotateX;
            Rotation.y -= rotate * Time.deltaTime * RotateSpeed;
            
            var f = Mathf.Pow(Friction, Time.deltaTime);

            var forwardVector = (Target - transform.position).normalized;
            forwardVector = Vector3.ProjectOnPlane(forwardVector, Vector3.up);
            var rightVector = Vector3.Cross(Vector3.up, forwardVector);
            Velocity -= forwardVector * _forward * Speed * Time.deltaTime;
            Velocity -= rightVector * _right * Speed * Time.deltaTime;
            Target += Velocity;
            Velocity *= f;
        }

        private void UpdateScroll()
        {
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
                c.orthographicSize = OrthographicSize;
            }
        }
    }
}