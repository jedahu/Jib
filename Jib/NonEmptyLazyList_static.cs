using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jib
{
    public sealed class NonEmptyLazyList<T> : ILazyList<T>, IUncons<T>
    {
        private readonly T head;
        private readonly Lazy<ILazyList<T>> tail;

        public NonEmptyLazyList(T head, Func<ILazyList<T>> tail)
        {
            this.head = head;
            this.tail = new Lazy<ILazyList<T>>(tail);
        }

        public T Head
        {
            get { return head; }
        }

        public ILazyList<T> Tail
        {
            get { return tail.Value; }
        }

        public Maybe<Tuple<T, ILazyList<T>>> Uncons
        {
            get { return Maybe.Just(Tuple.Create(Head, Tail)); }
        }

        public NonEmptyLazyList<T> Concat(IEnumerable<T> other)
        {
            return new NonEmptyLazyList<T>(head, () => tail.Value.Concat(other).ToLazyList());
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return head;
            foreach (var next in tail.Value)
            {
                yield return next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}