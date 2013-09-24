using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jib.Syntax
{
    public static class FuncFunctor
    {
        public static Func<X, B> Map<X, A, B>(this Func<A, B> f, Func<X, A> g)
        {
            return x => f(g(x));
        }
    }

    public static class MaybeFunctor
    {
        public static Maybe<B> Map<A, B>(this Maybe<A> maybe, Func<A, B> f)
        {
            return maybe.Cata(Maybe.Nothing<B>, a => Maybe.Just(f(a)));
        }
    }

    public static class EitherFunctor
    {
        public static Either<X, B> Map<X, A, B>(this Either<X, A> either, Func<A, B> f)
        {
            return either.Cata(Either.Left<X, B>, a => Either.Right<X, B>(f(a)));
        }

        public static Either<Y, A> MapRight<X, Y, A>(this Either<X, A> either, Func<X, Y> f)
        {
            return either.Swap(e => e.Map(f));
        }
    }

    public static class ValidationFunctor
    {
        public static Validation<X, B> Map<X, A, B>(this Validation<X, A> validation, Func<A, B> f)
        {
            return validation.Cata(Validation.Failure<X, B>, a => Validation.Success<X, B>(f(a)));
        }

        public static Validation<Y, A> MapFailure<X, Y, A>(this Validation<X, A> validation, Func<X, Y> f)
        {
            return validation.Cata(xs => Validation.Failure<Y, A>(xs.Map(f)), Validation.Success<Y, A>);
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

    public static class TaskFunctor
    {
        public static Task<B> Map<A, B>(this Task<A> task, Func<A, B> f)
        {
            return task.ContinueWith(t => f(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
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