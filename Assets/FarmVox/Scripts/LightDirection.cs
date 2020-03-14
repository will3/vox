using UnityEngine;

namespace FarmVox.Scripts
{
    public class LightDirection : MonoBehaviour
    {
        public Vector3Int lightDir = new Vector3Int(-1, -1, -1);

        private void Start()
        {
            transform.position = lightDir * -1;
            transform.LookAt(Vector3.zero, Vector3.up);
        }
    }
}