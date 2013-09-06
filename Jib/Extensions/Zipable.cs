using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jib.Extensions
{
    public static class MaybeZipable
    {
        public static Maybe<Tuple<A, B>> Zip<A, B>(this Maybe<A> maybe, Maybe<B> other)
        {
            return maybe.Cata(
                a => other.Cata(
                    b => Maybe.Just(Tuple.Create(a, b)),
                    Maybe.Nothing<Tuple<A, B>>),
                Maybe.Nothing<Tuple<A, B>>);
        }
    }

    public static class FutureZipable
    {
        public static Future<Tuple<A, B>> Zip<A, B>(this Future<A> future, Future<B> other)
        {
            return Future.Async<Tuple<A, B>>(cb =>
                {
                    var latch = new CountdownEvent(2);
                    var resA = default(A);
                    var resB = default(B);
                    Action next =
                        () =>
                            {
                                latch.Wait();
                                cb(Tuple.Create(resA, resB));
                            };

                    future.RunAsync(
                        a =>
                            {
                                resA = a;
                                latch.Signal();
                                next();
                            });
                    other.Callback(
                        b =>
                            {
                                resB = b;
                                latch.Signal();
                            });
                },
                future.Strategy);
        }
    }

    public static class EnumerableZipable
    {
        public static IEnumerable<Tuple<A, B>> Zip<A, B>(this IEnumerable<A> enumerable, IEnumerable<B> other)
        {
            var otherm = other.Memoize();
            return
                from a in enumerable
                from b in otherm
                select Tuple.Create(a, b);
        }
    }
}
