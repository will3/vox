using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace FarmVox
{
    public interface IWorker {
        void Start();
    }

    public class WorkerQueue
    {
        private HashSet<IWorker> workers = new HashSet<IWorker>();
        public System.DateTime nextStart;
        public int minWait = 10;

        public void Add(IWorker worker) {
            workers.Add(worker);
            StartAnyWorker();
        }

        public void Update() {
            StartAnyWorker();
        }

        public void StartAnyWorker() {
            if (System.DateTime.Now > nextStart)
            {
                if (workers.Count > 0) {
                    var worker = workers.ElementAt(0);
                    worker.Start();
                    workers.Remove(worker);
                    nextStart = System.DateTime.Now.AddMilliseconds(minWait);
                }
            }
        }
    }
}