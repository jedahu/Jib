using System;

namespace Jib
{
    public static class Arity
    {
        public delegate Func<TB, TR> CFunc<in TA, in TB, out TR>(TA a);

        public delegate Func<TB, Func<TC, TR>> CFunc<in TA, in TB, in TC, out TR>(TA a);

        #region Curry

        public static Func<TA, Func<TB, TR>> Curry<TA, TB, TR>(this Func<TA, TB, TR> func)
        {
            return a => b => func(a, b);
        }

        public static Func<TA, Func<TB, Func<TC, TR>>> Curry<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> func)
        {
            return a => b => c => func(a, b, c);
        }

        public static Func<TA, Func<TB, Func<TC, Func<TD, TR>>>> Curry<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> func)
        {
            return a => b => c => d => func(a, b, c, d);
        }

        public static Func<TA, Func<TB, Func<TC, Func<TD, Func<TE, TR>>>>> Curry<TA, TB, TC, TD, TE, TR>(this Func<TA, TB, TC, TD, TE, TR> func)
        {
            return a => b => c => d => e => func(a, b, c, d, e);
        }

        #endregion

        #region Uncurry

        public static Func<TA, TB, TR> Uncurry<TA, TB, TR>(this Func<TA, Func<TB, TR>> func)
        {
            return (a, b) => func(a)(b);
        }

        public static Func<TA, TB, TC, TR> Uncurry<TA, TB, TC, TR>(this Func<TA, Func<TB, Func<TC, TR>>> func)
        {
            return (a, b, c) => func(a)(b)(c);
        }

        public static Func<TA, TB, TC, TD, TR> Uncurry<TA, TB, TC, TD, TR>(this Func<TA, Func<TB, Func<TC, Func<TD, TR>>>> func)
        {
            return (a, b, c, d) => func(a)(b)(c)(d);
        }

        public static Func<TA, TB, TC, TD, TE, TR> Uncurry<TA, TB, TC, TD, TE, TR>(this Func<TA, Func<TB, Func<TC, Func<TD, Func<TE, TR>>>>> func)
        {
            return (a, b, c, d, e) => func(a)(b)(c)(d)(e);
        }

        #endregion

        #region Untuplize

        public static Func<TA, TB, TR> Untuplize<TA, TB, TR>(this Func<Tuple<TA, TB>, TR> func)
        {
            return (a, b) => func(Tuple.Create(a, b));
        }

        public static Func<TA, TB, TC, TR> Untuplize<TA, TB, TC, TR>(this Func<Tuple<TA, TB, TC>, TR> func)
        {
            return (a, b, c) => func(Tuple.Create(a, b, c));
        }

        public static Func<TA, TB, TC, TD, TR> Untuplize<TA, TB, TC, TD, TR>(this Func<Tuple<TA, TB, TC, TD>, TR> func)
        {
            return (a, b, c, d) => func(Tuple.Create(a, b, c, d));
        }

        public static Func<TA, TB, TC, TD, TE, TR> Untuplize<TA, TB, TC, TD, TE, TR>(this Func<Tuple<TA, TB, TC, TD, TE>, TR> func)
        {
            return (a, b, c, d, e) => func(Tuple.Create(a, b, c, d, e));
        }

        #endregion

        #region Tuplize

        public static Func<Tuple<TA, TB>, TR> Tuplize<TA, TB, TR>(this Func<TA, TB, TR> func)
        {
            return args => func(args.Item1, args.Item2);
        }

        public static Func<Tuple<TA, TB, TC>, TR> Tuplize<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3);
        }

        public static Func<Tuple<TA, TB, TC, TD>, TR> Tuplize<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4);
        }

        public static Func<Tuple<TA, TB, TC, TD, TE>, TR> Tuplize<TA, TB, TC, TD, TE, TR>(this Func<TA, TB, TC, TD, TE, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5);
        }

        #endregion
    }
}