using System;

namespace Jib
{
    public sealed class NonEmptyLazyList<A>
    {
        private readonly A head;
        private readonly Lazy<Maybe<NonEmptyLazyList<A>>> tail;

        public NonEmptyLazyList(A head, Func<Maybe<NonEmptyLazyList<A>>> tail)
        {
            this.head = head;
            this.tail = new Lazy<Maybe<NonEmptyLazyList<A>>>(tail);
        }

        public A Head
        {
            get { return head; }
        }

        public Maybe<NonEmptyLazyList<A>> Tail
        {
            get { return tail.Value; }
        }
    }
}