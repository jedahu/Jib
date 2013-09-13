using System;
using System.Collections.Generic;

namespace Jib.Extensions
{
    public static class EnumerableUncons
    {
        public static Maybe<Tuple<A, IEnumerable<A>>> Uncons<A>(this IEnumerable<A> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return Maybe.Just(Tuple.Create(enumerator.Current, enumerator.Enumerable()));
            }
            return Maybe.Nothing<Tuple<A, IEnumerable<A>>>();
        }
    }
}
