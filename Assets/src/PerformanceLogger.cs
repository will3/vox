using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class PerformanceLogger
    {
        static Stack<string> names = new Stack<string>();
        static Stack<DateTime> startTimes = new Stack<DateTime>();

        public static void Start(string name) {
            names.Push(name);
            startTimes.Push(System.DateTime.Now);
        }

        public static void End() {
            var end = DateTime.Now;
            var start = startTimes.Pop();
            var name = names.Pop();

            var diff = (end - start).Milliseconds;
            if (diff > 16) {
                Debug.Log(name + " took " + diff);
            }
        } 
    }
}