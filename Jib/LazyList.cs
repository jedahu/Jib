using System;
using System.Collections.Generic;

namespace Jib
{
    public static class LazyList
    {
        public static ILazyList<T> Empty<T>()
        {
            return new EmptyLazyList<T>();
        }

        public static ILazyList<T> ToLazyList<T>(this IEnumerator<T> enumerator)
        {
            return enumerator.MoveNext() ? new NonEmptyLazyList<T>(enumerator.Current, enumerator.ToLazyList) : Empty<T>();
        }

        public static ILazyList<T> ToLazyList<T>(this ILazyList<T> list)
        {
            return list;
        }

        public static ILazyList<T> ToLazyList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.GetEnumerator().ToLazyList();
        }

        public static Maybe<T> Head<T>(this ILazyList<T> list)
        {
            return ((IUncons<T>) list).Uncons.Map(u => u.Item1);
        }

        public static Maybe<ILazyList<T>> Tail<T>(this ILazyList<T> list)
        {
            return ((IUncons<T>) list).Uncons.Map(u => u.Item2);
        }

        public static Maybe<Tuple<T, ILazyList<T>>> Uncons<T>(this ILazyList<T> list)
        {
            return ((IUncons<T>) list).Uncons;
        }
    }
}