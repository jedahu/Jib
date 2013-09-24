using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jib.Syntax
{
    public static class MaybeApplicative
    {
        public static Maybe<A> PureMaybe<A>(this A value)
        {
            return Maybe.Just(value);
        }

        public static Maybe<B> Ap<A, B>(this Maybe<Func<A, B>> f, Maybe<A> arg)
        {
            return f.Bind(f0 => arg.Map(f0));
        }

        public static Maybe<B> Ap<A, B>(this Func<A, B> f, Maybe<A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class EitherApplicative
    {
        public static Either<A, X> PureEither<A, X>(this A value)
        {
            return Either.Left<A, X>(value);
        }

        public static Either<X, B> Ap<X, A, B>(this Either<X, Func<A, B>> f, Either<X, A> arg)
        {
            return f.Bind(f0 => arg.Map(f0));
        }

        public static Either<X, B> Ap<X, A, B>(this Func<A, B> f, Either<X, A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class ValidationApplicative
    {
        public static Validation<X, A> PureValidation<X, A>(this A value)
        {
            return Validation.Success<X, A>(value);
        }

        public static Validation<X, B> Ap<X, A, B>(this Validation<X, Func<A, B>> f, Validation<X, A> arg)
        {
            return f.Zip(arg).Map(p => p.Fst(p.Snd));
        }

        public static Validation<X, B> Ap<X, A, B>(this Func<A, B> f, Validation<X, A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class PromiseApplicative
    {
        public static Promise<A> PurePromise<A>(this A value)
        {
            var p = new Promise<A>();
            p.Signal(value);
            return p;
        }

        public static Promise<B> Ap<A, B>(this Promise<Func<A, B>> f, Promise<A> arg)
        {
            return arg.Map(f.Wait);
        }

        public static Promise<B> Ap<A, B>(this Func<A, B> f, Promise<A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class FutureApplicative
    {
        public static Future<A> PureFuture<A>(this A value)
        {
            return new Future<A>(cb => cb(value), Strategies.Id);
        }

        public static Future<B> Ap<A, B>(this Future<Func<A, B>> f, Future<A> arg)
        {
            return f.Zip(arg).Map(x => x.Fst(x.Snd));
        }

        public static Future<B> Ap<A, B>(this Func<A, B> f, Future<A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class TaskApplicative
    {
        public static Task<A> PureTask<A>(this A value)
        {
            return new Task<A>(() => value);
        }

        public static Task<B> Ap<A, B>(this Task<Func<A, B>> f, Task<A> arg)
        {
            return f.Zip(arg).Map(x => x.Fst(x.Snd));
        }

        public static Task<B> Ap<A, B>(this Func<A, B> f, Task<A> arg)
        {
            return arg.Map(f);
        }
    }

    public static class EnumerableApplicative
    {
        public static IEnumerable<A> PureEnumerable<A>(this A value)
        {
            yield return value;
        }

        public static IEnumerable<B> Ap<A, B>(this IEnumerable<Func<A, B>> f, IEnumerable<A> arg)
        {
            return f.SelectMany(arg.Select);
        }

        public static IEnumerable<B> Ap<A, B>(this Func<A, B> f, IEnumerable<A> arg)
        {
            return arg.Select(f);
        }
    }

    public static class NonEmptyLazyListApplicative
    {
        public static NonEmptyLazyList<A> PureNonEmptyLazyList<A>(this A value)
        {
            return NonEmptyLazyList.Single(value);
        }

        public static NonEmptyLazyList<B> Ap<A, B>(this NonEmptyLazyList<Func<A, B>> f, NonEmptyLazyList<A> arg)
        {
            return f.Bind(arg.Map);
        }

        public static NonEmptyLazyList<B> Ap<A, B>(this Func<A, B> f, NonEmptyLazyList<A> arg)
        {
            return arg.Map(f);
        }
    }
}
