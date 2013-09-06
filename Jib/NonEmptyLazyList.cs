using System.Collections.Generic;
using Jib.Extensions;

namespace Jib
{
    public static class NonEmptyLazyList
    {
        public static NonEmptyLazyList<T> Single<T>(T head)
        {
            return new NonEmptyLazyList<T>(head, LazyList.Empty<T>);
        }

        public static NonEmptyLazyList<T> Create<T>(T head, IEnumerable<T> tail)
        {
            return new NonEmptyLazyList<T>(head, tail.ToLazyList);
        }

        public static NonEmptyLazyList<T> Create<T>(T head, ILazyList<T> tail)
        {
            return new NonEmptyLazyList<T>(head, () => tail);
        }

        public static Maybe<NonEmptyLazyList<T>> MaybeNonEmptyLazyList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToLazyList().Uncons().Map(Arity.Tuplize<T, ILazyList<T>, NonEmptyLazyList<T>>(Create));
        }
    }
}