using System.Collections;
using System.Collections.Generic;

namespace FarmVox.Threading
{
    public class WorkerQueue
    {
        private readonly Queue<IWorker> _workers = new Queue<IWorker>();

        public void Enqueue(IWorker worker)
        {
            _workers.Enqueue(worker);
        }

        private IWorker Pop()
        {
            return _workers.Dequeue();
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
            _workers.Clear();
        }
    }
}