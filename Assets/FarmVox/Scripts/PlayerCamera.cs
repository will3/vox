using UnityEngine;

namespace FarmVox.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        public Camera Camera;

        public GameObject Target;
        
        public float Distance = 100;

        public float Pitch = 45;

        private void Start()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
            }

            if (Target == null)
            {
                Target = gameObject;
            }
        }

        private void Update()
        {
            var rotation = Target.transform.rotation;

            var angles = rotation.eulerAngles;
            angles.x = Pitch;
            
            var pos = Target.transform.position + Quaternion.Euler(angles) * Vector3.back * Distance;

            Camera.transform.position = pos;
            
            Camera.transform.LookAt(Target.transform);
        }
    }
}