using System;

namespace Jib
{
    public static class Arity
    {
        #region Curry

        public static Func<A, Func<B, R>> Curry<A, B, R>(this Func<A, B, R> func)
        {
            return a => b => func(a, b);
        }

        public static Func<A, Func<B, Func<C, R>>> Curry<A, B, C, R>(this Func<A, B, C, R> func)
        {
            return a => b => c => func(a, b, c);
        }

        public static Func<A, Func<B, Func<C, Func<D, R>>>> Curry<A, B, C, D, R>(this Func<A, B, C, D, R> func)
        {
            return a => b => c => d => func(a, b, c, d);
        }

        public static Func<A, Func<B, Func<C, Func<D, Func<E, R>>>>> Curry<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> func)
        {
            return a => b => c => d => e => func(a, b, c, d, e);
        }

        #endregion

        #region Uncurry

        public static Func<A, B, R> Uncurry<A, B, R>(this Func<A, Func<B, R>> func)
        {
            return (a, b) => func(a)(b);
        }

        public static Func<A, B, C, R> Uncurry<A, B, C, R>(this Func<A, Func<B, Func<C, R>>> func)
        {
            return (a, b, c) => func(a)(b)(c);
        }

        public static Func<A, B, C, D, R> Uncurry<A, B, C, D, R>(this Func<A, Func<B, Func<C, Func<D, R>>>> func)
        {
            return (a, b, c, d) => func(a)(b)(c)(d);
        }

        public static Func<A, B, C, D, E, R> Uncurry<A, B, C, D, E, R>(this Func<A, Func<B, Func<C, Func<D, Func<E, R>>>>> func)
        {
            return (a, b, c, d, e) => func(a)(b)(c)(d)(e);
        }

        #endregion

        #region Untuplize

        public static Func<A, B, R> Untuplize<A, B, R>(this Func<Tuple<A, B>, R> func)
        {
            return (a, b) => func(Tuple.Create(a, b));
        }

        public static Func<A, B, C, R> Untuplize<A, B, C, R>(this Func<Tuple<A, B, C>, R> func)
        {
            return (a, b, c) => func(Tuple.Create(a, b, c));
        }

        public static Func<A, B, C, D, R> Untuplize<A, B, C, D, R>(this Func<Tuple<A, B, C, D>, R> func)
        {
            return (a, b, c, d) => func(Tuple.Create(a, b, c, d));
        }

        public static Func<A, B, C, D, E, R> Untuplize<A, B, C, D, E, R>(this Func<Tuple<A, B, C, D, E>, R> func)
        {
            return (a, b, c, d, e) => func(Tuple.Create(a, b, c, d, e));
        }

        #endregion

        #region Tuplize

        public static Func<Tuple<A, B>, R> Tuplize<A, B, R>(this Func<A, B, R> func)
        {
            return args => func(args.Item1, args.Item2);
        }

        public static Func<Tuple<A, B, C>, R> Tuplize<A, B, C, R>(this Func<A, B, C, R> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3);
        }

        public static Func<Tuple<A, B, C, D>, R> Tuplize<A, B, C, D, R>(this Func<A, B, C, D, R> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4);
        }

        public static Func<Tuple<A, B, C, D, E>, R> Tuplize<A, B, C, D, E, R>(this Func<A, B, C, D, E, R> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5);
        }

        #endregion
    }
}