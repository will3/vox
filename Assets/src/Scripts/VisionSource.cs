using UnityEngine;

namespace FarmVox
{
    public class VisionSource : MonoBehaviour
    {
        public float radius = 200;
        public float blur = 40;

        void Start()
        {
            Vision.Instance.AddSource(this);
        }

        void OnDestroy()
        {
            Vision.Instance.RemoveSource(this);
        }

        void Update()
        {

        }
    }
}