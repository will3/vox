using System.Collections;
using System.Collections.Generic;

namespace FarmVox.Threading
{
    public class WorkerQueue
    {
        private readonly Queue<Worker> _workers = new Queue<Worker>();

        public void Enqueue(Worker worker)
        {
            _workers.Enqueue(worker);
        }

        public Worker Dequeue()
        {
            return _workers.Dequeue();
        }

        public IEnumerator DoWork()
        {
            while (true)
            {
                if (_workers.Count > 0)
                {
                    _workers.Dequeue().Start();
                }

                yield return null;
            }
        }
    }
}