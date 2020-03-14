using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class ValueGradient
    {
        public AnimationCurve Curve;

        public ValueGradient(float min, float max)
        {
            Curve = new AnimationCurve(new Keyframe(0.0f, min), new Keyframe(1.0f, max));
            
            for (var i = 0; i < Curve.keys.Length; i++)
            {
                Curve.SmoothTangents(i, 0);    
            }
        }

        public ValueGradient(Dictionary<float, float> map)
        {
            var keyframes = map.Select(u => new Keyframe(u.Key, u.Value)).ToArray();
            Curve = new AnimationCurve(keyframes);
            
            for (var i = 0; i < keyframes.Length; i++)
            {
                Curve.SmoothTangents(i, 0);    
            }
        }

        public float GetValue(float ratio)
        {
            return Curve.Evaluate(ratio);
        }
    }
}