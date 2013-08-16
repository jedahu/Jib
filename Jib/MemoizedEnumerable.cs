using System;
using System.Collections;
using System.Collections.Generic;

namespace Jib
{
    /// <summary>
    /// An enumerable that memoizes its items so it can be enumerated more than once with
    /// less performance penalty. The MemoizedEnumerable static class contains a Memoize
    /// extension method that takes an enumerable and returns a IMemoizedEnumerable.
    /// </summary>
    public class MemoizedEnumerable<T> : IMemoizedEnumerable<T>
    {
        private readonly IList<T> buffer = new List<T>();
        private readonly IEnumerator<T> source;
        private bool fullyEnumerated;

        public MemoizedEnumerable(IEnumerator<T> enumerator)
        {
            source = enumerator;
        }

        public IEnumerable<T> AsEnumerable
        {
            get { return this; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return fullyEnumerated ? GetBufferEnumerator() : GetMemoizingEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<T> GetBufferEnumerator()
        {
            return new BufferEnumerator(this);
        }

        private IEnumerator<T> GetMemoizingEnumerator()
        {
            return new MemoizingEnumerator(this);
        }

        private class BufferEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> peer;

            public BufferEnumerator(MemoizedEnumerable<T> me)
            {
                peer = me.buffer.GetEnumerator();
            }

            public void Dispose()
            {
                peer.Dispose();
            }

            public bool MoveNext()
            {
                return peer.MoveNext();
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public T Current
            {
                get { return peer.Current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private class MemoizingEnumerator : IEnumerator<T>
        {
            private readonly MemoizedEnumerable<T> me;
            private int index = -1;

            public MemoizingEnumerator(MemoizedEnumerable<T> me)
            {
                this.me = me;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                index = index + 1;
                var bufferNext = index < me.buffer.Count;
                if (!bufferNext && !me.fullyEnumerated)
                {
                    var sourceNext = me.source.MoveNext();
                    if (sourceNext) me.buffer.Add(me.source.Current);
                    else me.fullyEnumerated = true;
                    return sourceNext;
                }
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public T Current
            {
                get
                {
                    if (index >= me.buffer.Count)
                        throw new InvalidOperationException("`IEnumerator.Current` called on finished enumerator.");
                    return me.buffer[index];
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }

    public static class MemoizedEnumerable
    {
        /// <summary>
        /// Take an enumerable and return a memoized enumerable which can be enumerated
        /// multiple times but enumerates the original enumerable only once.
        /// </summary>
        public static IMemoizedEnumerable<T> Memoize<T>(this IEnumerable<T> enumerable)
        {
            return new MemoizedEnumerable<T>(enumerable.GetEnumerator());
        }
    }
}