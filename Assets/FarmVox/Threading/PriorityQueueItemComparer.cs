using System.Collections.Generic;

namespace FarmVox.Threading
{
    public class PriorityQueueItemComparer : IComparer<IPriorityQueueItem>
    {
        public int Compare(IPriorityQueueItem x, IPriorityQueueItem y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            
            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }
            
            return x.Priority.CompareTo(y.Priority);
        }
    }
}