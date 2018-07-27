using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class VisionSource : MonoBehaviour
    {
        public float radius = 100;
        public float blur = 20;

        void Start()
        {
            Finder.FindGameController().VisionMap.Add(this);
        }

        void OnDestroy()
        {
            var gameController = Finder.FindGameController();
            if (gameController != null) {
                gameController.VisionMap.Remove(this);    
            }
        }

        void Update()
        {

        }
    }
}