using System;
using Jib.Syntax;

namespace Jib
{
    public class Lens<A, B>
    {
        private readonly A record;
        private readonly Func<A, B> get;
        private readonly Func<A, B, A> set;

        public Lens(A record, Func<A, B> get, Func<A, B, A> set)
        {
            this.record = record;
            this.get = get;
            this.set = set;
        }

        public B Get
        {
            get { return get(record); }
        }

        public A Set(B a)
        {
            return set(record, a);
        }

        public A Update(Func<B, B> f)
        {
            return set(record, f(get(record)));
        }

        public Lens<A, C> Compose<C>(Lens<B, C> lens)
        {
            return new Lens<A, C>(
                record,
                lens.get.Map(get),
                (r, a) => set(r, lens.set(get(r), a)));
        }
    }

    public static class Lens
    {
        public static Lens<A, B> Create<A, B>(A record, Func<A, B> get, Func<A, B, A> set)
        {
            return new Lens<A, B>(record, get, set);
        }

    }

    public static class LensExtensions
    {
        public static Lens<A, B> Lens<A, B>(this A record, Func<A, B> get, Func<A, B, A> set)
        {
            return Jib.Lens.Create(record, get, set);
        }
    }
}