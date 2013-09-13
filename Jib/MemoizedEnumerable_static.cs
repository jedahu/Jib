using System.Collections.Generic;

namespace Jib
{
    public static class MemoizedEnumerable
    {
        /// <summary>
        /// Take an enumerable and return a memoized enumerable which can be enumerated
        /// multiple times but enumerates the original enumerable only once.
        /// </summary>
        public static MemoizedEnumerable<T> Memoize<T>(this IEnumerable<T> enumerable)
        {
            return new MemoizedEnumerable<T>(enumerable.GetEnumerator());
        }
    }
}