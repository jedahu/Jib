using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jib.Syntax
{
    public static class MaybeBind
    {
        public static Maybe<B> Bind<A, B>(this Maybe<A> maybe, Func<A, Maybe<B>> f)
        {
            return maybe.Cata(Maybe.Nothing<B>, f);
        }

        public static Maybe<T> Join<T>(this Maybe<Maybe<T>> maybe)
        {
            return maybe.Bind(a => a);
        }
    }

    public static class EitherBind
    {
        public static Either<X, B> Bind<X, A, B>(this Either<X, A> either, Func<A, Either<X, B>> f)
        {
            return either.Cata(Either.Left<X, B>, f);
        }

        public static Either<Y, A> BindLeft<X, Y, A>(this Either<X, A> either, Func<X, Either<Y, A>> f)
        {
            return either.Cata(f, Either.Right<Y, A>);
        }

        public static Either<X, A> Join<X, A>(this Either<X, Either<X, A>> either)
        {
            return either.Bind(a => a);
        }

        public static Either<X, A> JoinLeft<X, A>(this Either<Either<X, A>, A> either)
        {
            return either.BindLeft(a => a);
        }
    }

    public static class ValidationBind
    {
        public static Validation<X, B> Bind<X, A, B>(this Validation<X, A> validation, Func<A, Validation<X, B>> f)
        {
            return validation.Cata(Validation.Failure<X, B>, f);
        }

        public static Validation<X, A> Join<X, A>(this Validation<X, Validation<X, A>> validation)
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

    public static class TaskBind
    {
        public static Task<B> Bind<A, B>(this Task<A> task, Func<A, Task<B>> f)
        {
            return task.ContinueWith(t => f(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion).Result;
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