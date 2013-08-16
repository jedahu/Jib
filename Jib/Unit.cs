using System;

namespace Jib
{
    public sealed class Unit
    {
        private Unit() { }

        public static Unit Void = new Unit();

        public static Func<Unit> Func(Action k)
        {
            return () => { k(); return Void; };
        }

        public static Func<A, Unit> Func<A>(Action<A> k)
        {
            return a => { k(a); return Void; };
        }

        public static Func<A, B, Unit> Func<A, B>(Action<A, B> k)
        {
            return (a, b) => { k(a, b); return Void; };
        }

        public static Action Action(Func<Unit> k)
        {
            return () => k();
        }

        public static Action<A> Action<A>(Func<A, Unit> k)
        {
            return a => k(a);
        }

        public static Action<A, B> Action<A, B>(Func<A, B, Unit> k)
        {
            return (a, b) => k(a, b);
        }
    }
}
