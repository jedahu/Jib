using System.Collections.Generic;
using System.Linq;

namespace Jib.Syntax
{
    public static class MaybeEnumerable
    {
        public static IEnumerable<A> Enumerable<A>(this Maybe<A> maybe)
        {
            return maybe.Cata(System.Linq.Enumerable.Empty<A>, a => a.PureEnumerable());
        }
    }

    public static class NonEmptyLazyListEnumerable
    {
        public static IEnumerable<A> Enumerable<A>(this NonEmptyLazyList<A> nel)
        {
            yield return nel.Head;
            foreach (var a in nel.Tail.Enumerable().SelectMany(tail => tail.Enumerable()))
            {
                yield return a;
            }
        }
    }

    public static class LazyListEnumerable
    {
        public static IEnumerable<A> Enumerable<A>(this LazyList<A> list)
        {
            foreach (var a in list.Head.Enumerable())
            {
                yield return a;
            }
            foreach (var a in list.Tail.Enumerable().SelectMany(tail => tail.Enumerable()))
            {
                yield return a;
            }
        }
    }

    public static class MemoizedEnumerableEnumerable
    {
        public static IEnumerable<A> Enumerable<A>(this MemoizedEnumerable<A> memo)
        {
            return memo;
        }
    }

    public static class EnumeratorEnumerable
    {
        public static IEnumerable<A> Enumerable<A>(this IEnumerator<A> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
    }
}
