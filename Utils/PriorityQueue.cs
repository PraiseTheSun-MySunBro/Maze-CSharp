using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly SortedList<int, T> _holder;

        public PriorityQueue()
        {
            _holder = new SortedList<int, T>();
        }

        public PriorityQueue(
            IComparer<int> comparer)
        {
            _holder = new SortedList<int, T>(comparer);
        }

        public int Count => _holder.Count;

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public T Peek()
        {
            if (IsEmpty())
            {
                return default;
            }

            return _holder.First().Value;
        }

        public void Enqueue(int priority, T node)
        {
            _holder.Add(priority, node);
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                return default;
            }

            var value = _holder.First().Value;
            _holder.RemoveAt(0);
            return value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var (_, value) in _holder)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MinPriorityQueue<T> : PriorityQueue<T>
    {
        private class MinPriorityComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                var compare = x.CompareTo(y);
                if (compare == 0)
                {
                    return 1;
                }
                return compare;
            }
        }

        public MinPriorityQueue() : base(new MinPriorityComparer())
        {
        }
    }

    public class MaxPriorityQueue<T> : PriorityQueue<T>
    {
        private class MaxPriorityComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                var compare = y.CompareTo(x);
                if (compare == 0)
                {
                    return 1;
                }
                return compare;
            }
        }

        public MaxPriorityQueue() : base(new MaxPriorityComparer())
        {
        }
    }
}
