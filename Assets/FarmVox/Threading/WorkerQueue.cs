using System.Collections;
using System.Collections.Generic;

namespace FarmVox.Threading
{
    public class WorkerQueue
    {
        private readonly PriorityQueue _workers = new PriorityQueue();

        public void Enqueue(IWorker worker)
        {
            _workers.Add(worker);
        }

        private IWorker Pop()
        {
            return _workers.Pop() as IWorker;
        }

        public IEnumerator DoWork()
        {
            while (true)
            {
                if (_workers.Count > 0)
                {
                    Pop().Start();
                }

                yield return null;
            }
        }

        public void RemoveAll()
        {
            _workers.RemoveAll();
        }
    }
}