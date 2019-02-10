using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class ActorSpriteSheet
    {
        public string[] Idle;
        
        public float IdleSpeed;

        public void AssertSpriteSheetValid()
        {
            if (Math.Abs(IdleSpeed) < Mathf.Epsilon)
            {
                throw new InvalidOperationException("IdleSpeed cannot be 0");
            }

            if (Idle.Length == 0)
            {
                throw new InvalidOperationException("Idle textures not configured");
            }
        }
    }
}