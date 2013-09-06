using System;
using System.Collections.Generic;
using System.Linq;
using Jib.Extensions;

namespace Jib
{
    public static class Maybe
    {
        public static Maybe<A> Just<A>(A value)
        {
            return new Maybe<A>(value);
        }

        public static Maybe<A> Nothing<A>()
        {
            return new Maybe<A>();
        }

        public static IEnumerable<A> Justs<A>(this IEnumerable<Maybe<A>> maybes)
        {
            return maybes.SelectMany(m => MaybeMorphisms.Enumerable<A>(m));
        }

        public static bool IsJust<A>(this Maybe<A> maybe)
        {
            return maybe.Cata(a => true, () => false);
        }

        public static bool IsNothing<A>(this Maybe<A> maybe)
        {
            return maybe.Cata(a => false, () => true);
        }

        public static A ValueOr<A>(this Maybe<A> maybe, Func<A> elseFunc)
        {
            return maybe.Cata(x => x, elseFunc);
        }
    }
}