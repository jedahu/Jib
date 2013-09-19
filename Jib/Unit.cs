using System;

namespace Jib
{
    public sealed class Unit
    {
        private Unit() { }

        public static Unit Void = new Unit();

        #region Action to Func

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

        public static Func<A, B, C, Unit> Func<A, B, C>(Action<A, B, C> k)
        {
            return (a, b, c) => { k(a, b, c); return Void; };
        }

        public static Func<A, B, C, D, Unit> Func<A, B, C, D>(Action<A, B, C, D> k)
        {
            return (a, b, c, d) => { k(a, b, c, d); return Void; };
        }

        public static Func<A, B, C, D, E, Unit> Func<A, B, C, D, E>(Action<A, B, C, D, E> k)
        {
            return (a, b, c, d, e) => { k(a, b, c, d, e); return Void; };
        }

        #endregion

        #region Func to Action

        public static Action Action<R>(Func<R> k)
        {
            return () => k();
        }

        public static Action<A> Action<A, R>(Func<A, R> k)
        {
            return a => k(a);
        }

        public static Action<A, B> Action<A, B, R>(Func<A, B, R> k)
        {
            return (a, b) => k(a, b);
        }

        public static Action<A, B, C> Action<A, B, C, R>(Func<A, B, C, R> k)
        {
            return (a, b, c) => k(a, b, c);
        }

        public static Action<A, B, C, D> Action<A, B, C, D, R>(Func<A, B, C, D, R> k)
        {
            return (a, b, c, d) => k(a, b, c, d);
        }

        public static Action<A, B, C, D, E> Action<A, B, C, D, E, R>(Func<A, B, C, D, E, R> k)
        {
            return (a, b, c, d, e) => k(a, b, c, d, e);
        }

        #endregion
    }
}
