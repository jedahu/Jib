using System;
using System.Linq;

namespace Jib.Extensions
{
    public static class MaybeBind
    {
        public static Maybe<B> Bind<A, B>(this Maybe<A> maybe, Func<A, Maybe<B>> f)
        {
            return maybe.Cata(f, Maybe.Nothing<B>);
        }

        public static Maybe<T> Join<T>(this Maybe<Maybe<T>> maybe)
        {
            return maybe.Bind(a => a);
        }
    }

    public static class EitherBind
    {
        public static Either<B, X> Bind<A, B, X>(this Either<A, X> either, Func<A, Either<B, X>> f)
        {
            return either.Cata(f, Either.Right<B, X>);
        }

        public static Either<A, Y> BindRight<A, X, Y>(this Either<A, X> either, Func<X, Either<A, Y>> f)
        {
            return either.Cata(Either.Left<A, Y>, f);
        }

        public static Either<A, X> Join<A, X>(this Either<Either<A, X>, X> either)
        {
            return either.Bind(a => a);
        }

        public static Either<A, X> JoinRight<A, X>(this Either<A, Either<A, X>> either)
        {
            return either.BindRight(a => a);
        }
    }

    public static class ValidationBind
    {
        public static Validation<B, X> Bind<A, B, X>(this Validation<A, X> validation, Func<A, Validation<B, X>> f)
        {
            return validation.Cata(f, Validation.Failure<B, X>);
        }

        public static Validation<A, X> Join<A, X>(this Validation<Validation<A, X>, X> validation)
        {
            return validation.Bind(a => a);
        }
    }

    public static class PromiseBind
    {
        public static Promise<B> Bind<A, B>(this Promise<A> promise, Func<A, Promise<B>> f)
        {
            return promise.Map(a => f(a).Wait);
        }

        public static Promise<T> Join<T>(this Promise<Promise<T>> promise)
        {
            return promise.Bind(a => a);
        }
    }

    public static class FutureBind
    {
        public static Future<B> Bind<A, B>(this Future<A> future, Func<A, Future<B>> f)
        {
            return new Future<B>(
                cb => future.Callback(a => f(a).Callback(cb)),
                future.Strategy);
        }

        public static Future<T> Join<T>(this Future<Future<T>> future)
        {
            return future.Bind(a => a);
        }
    }

    public static class NonEmptyLazyListBind
    {
        public static NonEmptyLazyList<B> Bind<A, B>(this NonEmptyLazyList<A> list, Func<A, NonEmptyLazyList<B>> f)
        {
            var ht = list.HeadTail();
            return f(ht.Fst).SemiOp(ht.Snd.Select(f).Aggregate((a, b) => a.SemiOp(b)));
        }

        public static NonEmptyLazyList<A> Join<A>(this NonEmptyLazyList<NonEmptyLazyList<A>> list)
        {
            return list.Bind(a => a);
        }
    }
}