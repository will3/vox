using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace FarmVox
{
    public abstract class Worker {
        public abstract void Start();
    }

    public class WorkerQueue
    {
        List<Worker> workers = new List<Worker>();

        public void Add(Worker worker) {
            workers.Add(worker);
        }

        public void Update() {
            StartOne();
        }

        public void StartOne() {
            if (workers.Count > 0) {
                var worker = workers[0];
                worker.Start();
                workers.Remove(worker);    
            }
        }
    }
}