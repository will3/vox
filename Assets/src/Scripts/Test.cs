using UnityEngine;
using System.Collections;
using System;

namespace FarmVox
{
    public class Test : MonoBehaviour
    {
        private Perlin3DGPU noise;

        // Use this for initialization
        void Start()
        {
            var start = DateTime.Now;
            noise = new Perlin3DGPU(32);
            noise.Dispatch();
            var results = noise.Read();
            noise.ReleaseBuffer();
            var end = DateTime.Now;
            Debug.Log((end - start).Milliseconds);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}