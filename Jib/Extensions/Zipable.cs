﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jib.Extensions
{
    public static class MaybeZipable
    {
        public static Maybe<Pair<A, B>> Zip<A, B>(this Maybe<A> maybe, Maybe<B> other)
        {
            return maybe.Cata(
                a => other.Cata(
                    b => Maybe.Just(Pair.Create(a, b)),
                    Maybe.Nothing<Pair<A, B>>),
                Maybe.Nothing<Pair<A, B>>);
        }
    }

    public static class ValidationZipable
    {
        public static Validation<Pair<A, B>, X> Zip<A, B, X>(this Validation<A, X> validation, Validation<B, X> other)
        {
            return validation.Cata(
                a => other.Cata(
                    b => Validation.Success<Pair<A, B>, X>(Pair.Create(a, b)),
                    Validation.Failure<Pair<A, B>, X>),
                xs1 => other.Cata(
                    b => Validation.Failure<Pair<A, B>, X>(xs1),
                    xs2 => Validation.Failure<Pair<A, B>, X>(xs1.SemiOp(xs2))));
        }
    }

    public static class FutureZipable
    {
        public static Future<Pair<A, B>> Zip<A, B>(this Future<A> future, Future<B> other)
        {
            return Future.Async<Pair<A, B>>(cb =>
                {
                    var latch = new CountdownEvent(2);
                    var resA = default(A);
                    var resB = default(B);
                    Action next =
                        () =>
                            {
                                latch.Wait();
                                cb(Pair.Create(resA, resB));
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
        public static IEnumerable<Pair<A, B>> Zip<A, B>(this IEnumerable<A> enumerable, IEnumerable<B> other)
        {
            var otherm = other.Memoize();
            return
                from a in enumerable
                from b in otherm
                select Pair.Create(a, b);
        }
    }
}
