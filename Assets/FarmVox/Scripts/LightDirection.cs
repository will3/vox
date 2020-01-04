using UnityEngine;

namespace FarmVox.Scripts
{
    public class LightDirection : MonoBehaviour
    {
        public Vector3 pos;

        private void Start()
        {
            transform.position = pos;
            transform.LookAt(Vector3.zero, Vector3.up);
        }
    }
}