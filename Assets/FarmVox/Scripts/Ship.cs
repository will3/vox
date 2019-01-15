using UnityEngine;

namespace FarmVox.Scripts
{
    public class Ship : MonoBehaviour
    {
        private Vector3 _velocity;
        public float Power = 1;
        public float Friction = 0.001f;

        public float Yaw;
        public float Pitch;
        public float Roll;
        
        private void Update()
        {
            var forward = 0.0f;
            var right = 0.0f;
            
            if (Input.GetKey(KeyCode.W))
            {
                forward += 1.0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                forward -= 1.0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                right += 1.0f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                right -= 1.0f;
            }

            if (right != 0.0f)
            {
                forward = 1.0f;
            }

            var maxRoll = 45;
            Roll -= right * 10f;
            Roll *= 0.9f;
            Roll = Mathf.Clamp(Roll, -maxRoll, maxRoll);

            Yaw -= Mathf.Sin(Roll * Mathf.Deg2Rad) * 2f;
            
            _velocity += transform.rotation * Vector3.forward * forward * Power * Time.deltaTime;

            var f = Mathf.Pow(Friction, Time.deltaTime);

            _velocity *= f;
            
            transform.position += _velocity;

            var rotation = Quaternion.AngleAxis(Yaw, Vector3.up) *
                       Quaternion.AngleAxis(Pitch, Vector3.right) *
                       Quaternion.AngleAxis(Roll, Vector3.forward);

            transform.rotation = rotation;
        }
    }
}