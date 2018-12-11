namespace FarmVox.Threading
{
    public interface IWorker : IPriorityQueueItem
    {
        void Start();
    }
}