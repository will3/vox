using System.Collections.Generic;
using System.Threading;

namespace FarmVox
{
    public class Worker {
        public bool done = false;
        public void Start() {
            
        }

        public void onDone() {
            
        }
    }

    public class WorkerQueue
    {
        private HashSet<Worker> workers;

        public void Add(Worker worker) {
            workers.Add(worker);

            Thread thread = new Thread(worker.Start);
            thread.Start();
        }

        public void Update() {
            var copy = new HashSet<Worker>(workers);

            foreach(var worker in copy) {
                if (worker.done) {
                    worker.onDone();
                    workers.Remove(worker);
                }
            }
        }
    }
}