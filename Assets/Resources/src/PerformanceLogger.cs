using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class PerformanceLogger
    {
        static Stack<string> names = new Stack<string>();
        static Stack<DateTime> startTimes = new Stack<DateTime>();
        static Stack<int> thresholds = new Stack<int>();

        public static void Push(string name, int threshold = 16) {
            names.Push(name);
            startTimes.Push(DateTime.Now);
            thresholds.Push(threshold);
        }

        public static void Pop() {
            var end = DateTime.Now;
            var start = startTimes.Pop();
            var name = names.Pop();
            var threshold = thresholds.Pop();

            var diff = (end - start).Milliseconds;
            if (diff > threshold) {
                Debug.Log(name + " took " + diff);
            }
        }
    }
}