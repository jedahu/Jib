using System.Collections.Generic;
using System.Linq;

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

    public static class NonEmptyLazyListUncons
    {
        public static Maybe<Pair<A, IEnumerable<A>>> Uncons<A>(this NonEmptyLazyList<A> list)
        {
            return Maybe.Just(list.HeadTail());
        }
    }
}
