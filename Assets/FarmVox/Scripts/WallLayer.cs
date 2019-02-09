using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class WallLayer : MonoBehaviour
    {
        public static WallLayer Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Only one WallLayer should be created");
            }
            
            Instance = this;
        }

        public void AddWall(Vector3Int coord)
        {
            
        }
    }
}