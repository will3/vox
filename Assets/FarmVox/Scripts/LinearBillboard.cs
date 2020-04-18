using UnityEngine;

namespace FarmVox.Scripts
{
    public class LinearBillboard : MonoBehaviour
    {
        public Camera targetCamera;
        public Vector3 scale = new Vector3(12, 18, 12);
        public float scaleMultiplier = 1.0f;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            transform.localScale = scale * scaleMultiplier;

            var forward = targetCamera.transform.rotation * Vector3.forward;
            var right = Vector3.Cross(Vector3.up, forward);
            var face = Vector3.Cross(right, Vector3.up);

            transform.LookAt(transform.position + face, Vector3.up);
        }
    }
}