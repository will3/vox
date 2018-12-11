using System.Collections.Generic;
using System.Linq;

namespace FarmVox.Threading
{
    public class PriorityQueue
    {
        private readonly PriorityQueueItemComparer _comparer = new PriorityQueueItemComparer();
        
        private readonly List<IPriorityQueueItem> _list = new List<IPriorityQueueItem>();

        public void Add(IPriorityQueueItem obj)
        {
            var index = _list.BinarySearch(0, _list.Count, obj, _comparer);

            if (index < 0)
            {
                _list.Insert(~index, obj);    
            }
            else
            {
                _list.Insert(index, obj);
            }
        }

        public IPriorityQueueItem Pop()
        {
            if (_list.Count == 0)
            {
                return default(IPriorityQueueItem);
            }
            
            var last = _list.Last();
            _list.RemoveAt(_list.Count - 1);
            return last;
        }

        public int Count
        {
            get { return _list.Count; }
        }
    }
}