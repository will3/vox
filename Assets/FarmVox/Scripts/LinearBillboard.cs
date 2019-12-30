using UnityEngine;

namespace FarmVox.Scripts
{
    public class LinearBillboard : MonoBehaviour
    {
        public Camera targetCamera;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }
        private void Update()
        {
            var forward = targetCamera.transform.rotation * Vector3.forward;
            var right = Vector3.Cross(Vector3.up, forward);
            var face = Vector3.Cross(right, Vector3.up);

            transform.LookAt(transform.position + face, Vector3.up);
        }
    }
}