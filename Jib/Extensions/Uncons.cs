using System.Collections.Generic;

namespace Jib.Extensions
{
    public static class EnumerableUncons
    {
        public static Maybe<Pair<A, IEnumerable<A>>> Uncons<A>(this IEnumerable<A> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return Maybe.Just(Pair.Create(enumerator.Current, enumerator.Enumerable()));
            }
            return Maybe.Nothing<Pair<A, IEnumerable<A>>>();
        }
    }
}
