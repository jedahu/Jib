using System;
using System.Linq;

namespace Jib.Extensions
{
    public static class MaybeFunctor
    {
        public static Maybe<B> Map<A, B>(this Maybe<A> maybe, Func<A, B> f)
        {
            return maybe.Cata(
                a => Maybe.Just(f(a)),
                Maybe.Nothing<B>);
        }
    }

    public static class EitherFunctor
    {
        public static Either<B, X> Map<A, B, X>(this Either<A, X> either, Func<A, B> f)
        {
            return either.Cata(a => Either.Left<B, X>(f(a)), Either.Right<B, X>);
        }

        public static Either<A, Y> MapRight<A, X, Y>(this Either<A, X> either, Func<X, Y> f)
        {
            return either.Swap(e => e.Map(f));
        }
    }

    public static class ValidationFunctor
    {
        public static Validation<B, X> Map<A, B, X>(this Validation<A, X> validation, Func<A, B> f)
        {
            return validation.Cata(a => Validation.Success<B, X>(f(a)), Validation.Failure<B, X>);
        }

        public static Validation<A, Y> MapFailure<A, X, Y>(this Validation<A, X> validation, Func<X, Y> f)
        {
            return validation.Cata(Validation.Success<A, Y>, xs => Validation.Failure<A, Y>(xs.Map(f)));
        }
    }

    public static class PromiseFunctor
    {
        public static Promise<B> Map<A, B>(this Promise<A> promise, Func<A, B> f)
        {
            var p = new Promise<B>();
            p.Strategy.Call(() => p.Signal(f(promise.Wait)));
            return p;
        }
    }

    public static class FutureFunctor
    {
        public static Future<B> Map<A, B>(this Future<A> future, Func<A, B> f)
        {
            return new Future<B>(
                cb => future.Callback(a => cb(f(a))),
                future.Strategy);
        }
    }

    public static class NonEmptyLazyListFunctor
    {
        public static NonEmptyLazyList<B> Map<A, B>(this NonEmptyLazyList<A> list, Func<A, B> f)
        {
            var ht = list.HeadTail();
            return f(ht.Fst).Cons(ht.Snd.Select(f));
        }
    }
}