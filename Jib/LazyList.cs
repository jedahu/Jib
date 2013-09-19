using System;
using Jib.Extensions;

namespace Jib
{
    public class LazyList<A>
        : Unobject
    {
        private readonly Maybe<Tuple<A, Lazy<LazyList<A>>>> data;

        public LazyList()
        {
            data = Maybe.Nothing<Tuple<A, Lazy<LazyList<A>>>>();
        }

        public LazyList(A head, Func<LazyList<A>> tail)
        {
            data = Maybe.Just(Tuple.Create(head, new Lazy<LazyList<A>>(tail)));
        }

        public Maybe<A> Head
        {
            get { return data.Map(d => d.Item1); }
        }

        public Maybe<LazyList<A>> Tail
        {
            get { return data.Map(d => d.Item2.Value); }
        }
    }
}
