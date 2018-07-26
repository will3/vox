using UnityEngine;

namespace FarmVox
{
    public class VisionSource : MonoBehaviour
    {
        public int radius = 10;

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