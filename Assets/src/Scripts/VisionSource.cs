using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class VisionSource : MonoBehaviour
    {
        public float radius = 75;
        public float blur = 20;
        public bool moved = false;
        public Vector3 lastPosition;

        void Start()
        {
            Finder.FindGameController().VisionMap.Add(this);
            lastPosition = transform.position;
        }

        void Update()
        {
            moved = transform.position != lastPosition;

            lastPosition = transform.position;
        }

        void OnDestroy()
        {
            var gameController = Finder.FindGameController();
            if (gameController != null)
            {
                gameController.VisionMap.Remove(this);
            }
        }
    }
}