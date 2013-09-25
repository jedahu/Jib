using System;
using Jib.Syntax;

namespace Jib
{
    public class Lens<A, B>
    {
        private readonly Func<A, B> get;
        private readonly Func<A, B, A> set;

        public Lens(Func<A, B> get, Func<A, B, A> set)
        {
            this.get = get;
            this.set = set;
        }

        public B Get(A a)
        {
            return get(a);
        }

        public A Set(A a, B b)
        {
            return set(a, b);
        }

        public A Update(A a, Func<B, B> f)
        {
            return set(a, f(get(a)));
        }

        public Lens<A, C> Compose<C>(Lens<B, C> lens)
        {
            return new Lens<A, C>(
                lens.get.Map(get),
                (a, c) => set(a, lens.set(get(a), c)));
        }
    }

    public static class Lens
    {
        public static Lens<A, B> Create<A, B>(Func<A, B> get, Func<A, B, A> set)
        {
            return new Lens<A, B>(get, set);
        }
    }
}