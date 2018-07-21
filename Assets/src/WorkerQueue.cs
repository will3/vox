using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace FarmVox
{
    public interface IWorker {
        void Start();
        bool IsDone();
    }

    public class WorkerQueue
    {
        private HashSet<IWorker> workers = new HashSet<IWorker>();
        public System.DateTime nextStart;
        public int minWait = 10;
        private IWorker currentWorker;
        public void Add(IWorker worker) {
            workers.Add(worker);
            //StartAnyWorker();
        }

        public void Update() {
            if (currentWorker != null && currentWorker.IsDone()) {
                currentWorker = null;
            }
            StartAnyWorker();
        }

        public void StartAnyWorker() {
            if (currentWorker != null) {
                return;
            }
            if (System.DateTime.Now > nextStart)
            {
                if (workers.Count > 0) {
                    var worker = workers.ElementAt(0);
                    worker.Start();
                    currentWorker = worker;
                    workers.Remove(worker);
                    nextStart = System.DateTime.Now.AddMilliseconds(minWait);
                }
            }
        }
    }
}