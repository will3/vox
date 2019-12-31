using System;
using FarmVox.Threading;

namespace FarmVox.Workers
{
    public class ActionWorker : IWorker
    {
        private readonly Action _action;

        public ActionWorker(Action action)
        {
            _action = action;
        }

        public void Start()
        {
            _action();
        }
    }
}