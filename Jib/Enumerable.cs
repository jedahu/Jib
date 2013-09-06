using System;
using System.Collections.Generic;
using System.Linq;
using Jib.Extensions;

namespace Jib
{
    public static class EnumerableTraversable
    {
        //public static Maybe<IEnumerable<TB>> TraverseMaybe<TA, TB>(this IEnumerable<TA> enumerable, Func<TA, Maybe<TB>> f)
        //{
        //    return enumerable.ToLazyList().TraverseMaybe(f).Map<IEnumerable<TB>>(l => l);
        //}

        //public static Maybe<IEnumerable<TA>> SequenceMaybe<TA>(this IEnumerable<Maybe<TA>> enumerable)
        //{
        //    return enumerable.TraverseMaybe(a => a);
        //}

        public static TA Reduce<TA>(this IEnumerable<TA> enumerable, IMonoid<TA> monoid)
        {
            return enumerable.Aggregate(monoid.Zero, monoid.Op);
        }

        public static TB ReduceMap<TA, TB>(this IEnumerable<TA> enumerable, Func<TA, TB> f, IMonoid<TB> monoid)
        {
            return enumerable.Select(f).Aggregate(monoid.Zero, monoid.Op);
        }
    }
}
