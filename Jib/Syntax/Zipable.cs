using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jib.Instances;

namespace Jib.Syntax
{
    public static class MaybeZipableSyntax
    {
        public static Maybe<Pair<A, B>> Zip<A, B>(this Maybe<A> maybe, Maybe<B> other)
        {
            return MaybeZipable.Instance.Zip(maybe, other);
        }
    }

    public static class ValidationZipableSyntax
    {
        public static Validation<X, Pair<A, B>> Zip<X, A, B>(this Validation<X, A> validation, Validation<X, B> other)
        {
            return validation.Cata(
                xs1 => other.Cata(
                    xs2 => Validation.Failure<X, Pair<A, B>>(xs1.SemiOp(xs2)),
                    b => Validation.Failure<X, Pair<A, B>>(xs1)),
                a => other.Cata(Validation.Failure<X, Pair<A, B>>,
                                b =>
                                Validation.Success<X, Pair<A, B>>(
                                    Pair.Create(a, b))));
        }
    }

    public static class FutureZipableSyntax
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

    public static class TaskZipableSyntax
    {
        public static Task<Pair<A, B>> Zip<A, B>(this Task<A> task, Task<B> other)
        {
            return
                Task.WhenAll(new Task[] {task, other})
                    .ContinueWith(
                        t => Pair.Create(task.Result, other.Result),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }

    public static class EnumerableZipableSyntax
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

    public static class NonEmptyLazyListZipableSyntax
    {
        public static NonEmptyLazyList<Pair<A, B>> Zip<A, B>(this NonEmptyLazyList<A> list, NonEmptyLazyList<B> other)
        {
            return
                from a in list
                from b in other
                select Pair.Create(a, b);
        }
    }
}
