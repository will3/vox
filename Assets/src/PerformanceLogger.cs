using System;
using UnityEngine;

namespace FarmVox
{
    public static class PerformanceLogger
    {
        static string startName;
        static DateTime start;

        public static void Start(string name) {
            if (startName != null)
            {
                Debug.LogWarning("Starting another performance log. Make sure to call End()");
            }
            start = DateTime.Now;
            startName = name;
        }

        public static void End() {
            var end = DateTime.Now;
            var diff = (end - start).Milliseconds;
            if (diff > 16) {
                Debug.Log(startName + " took " + diff);
            }
            startName = null;
        } 
    }
}