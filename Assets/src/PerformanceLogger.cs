using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public static class PerformanceLogger
    {
        private static readonly Stack<string> Names = new Stack<string>();
        private static readonly Stack<DateTime> StartTimes = new Stack<DateTime>();
        private static readonly Stack<int> Thresholds = new Stack<int>();

        public static void Push(string name, int threshold = 16) {
            Names.Push(name);
            StartTimes.Push(DateTime.Now);
            Thresholds.Push(threshold);
        }

        public static void Pop() {
            var end = DateTime.Now;
            var start = StartTimes.Pop();
            var name = Names.Pop();
            var threshold = Thresholds.Pop();

            var diff = (end - start).Milliseconds;
            if (diff > threshold) {
                Debug.Log(name + " took " + diff);
            }
        }
    }
}