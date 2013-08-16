using System;

namespace Jib
{
    public static class Arity
    {
        public static Func<Tuple<TA, TB>, TR> Uncurry<TA, TB, TR>(this Func<TA, TB, TR> func)
        {
            return args => func(args.Item1, args.Item2);
        }

        public static Func<Tuple<TA, TB, TC>, TR> Uncurry<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3);
        }

        public static Func<Tuple<TA, TB, TC, TD>, TR> Uncurry<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4);
        }

        public static Func<Tuple<TA, TB, TC, TD, TE>, TR> Uncurry<TA, TB, TC, TD, TE, TR>(this Func<TA, TB, TC, TD, TE, TR> func)
        {
            return args => func(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5);
        }

        public static Func<TA, TB, TR> Curry<TA, TB, TR>(this Func<Tuple<TA, TB>, TR> func)
        {
            return (a, b) => func(Tuple.Create(a, b));
        }

        public static Func<TA, TB, TC, TR> Curry<TA, TB, TC, TR>(this Func<Tuple<TA, TB, TC>, TR> func)
        {
            return (a, b, c) => func(Tuple.Create(a, b, c));
        }

        public static Func<TA, TB, TC, TD, TR> Curry<TA, TB, TC, TD, TR>(this Func<Tuple<TA, TB, TC, TD>, TR> func)
        {
            return (a, b, c, d) => func(Tuple.Create(a, b, c, d));
        }

        public static Func<TA, TB, TC, TD, TE, TR> Curry<TA, TB, TC, TD, TE, TR>(this Func<Tuple<TA, TB, TC, TD, TE>, TR> func)
        {
            return (a, b, c, d, e) => func(Tuple.Create(a, b, c, d, e));
        }
    }
}