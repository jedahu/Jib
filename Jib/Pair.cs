using System;

namespace Jib
{
    public struct Pair<A, B>
    {
        public readonly A Fst;

        public readonly B Snd;

        public Pair(A a, B b)
        {
            this.Fst = a;
            this.Snd = b;
        }
    }

    public static class Pair
    {
        public static Func<Pair<A, B>, X> UnCurry<A, B, X>(this Func<A, B, X> f)
        {
            return p => f(p.Fst, p.Snd);
        }

        public static Func<Pair<A, Pair<B, C>>, X> UnCurry<A, B, C, X>(this Func<A, B, C, X> f)
        {
            return p => f(p.Fst, p.Snd.Fst, p.Snd.Snd);
        }

        public static Action<Pair<A, B>> UnCurry<A, B>(this Action<A, B> a)
        {
            return p => a(p.Fst, p.Snd);
        }

        public static Action<Pair<A, Pair<B, C>>> UnCurry<A, B, C>(this Action<A, B, C> a)
        {
            return p => a(p.Fst, p.Snd.Fst, p.Snd.Snd);
        }
    }
}
