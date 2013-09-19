using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Extensions
{
    public static class MaybeApplicative
    {
        public static Maybe<T> PureMaybe<T>(this T value)
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

        public static Either<B, X> Ap<A, B, X>(this Either<Func<A, B>, X> f, Either<A, X> arg)
        {
            return f.Bind(f0 => arg.Map(f0));
        }

        public static Either<B, X> Ap<A, B, X>(this Func<A, B> f, Either<A, X> arg)
        {
            return arg.Map(f);
        }
    }

    public static class ValidationApplicative
    {
        public static Validation<A, X> PureValidation<A, X>(this A value)
        {
            return Validation.Success<A, X>(value);
        }

        public static Validation<B, X> Ap<A, B, X>(this Validation<Func<A, B>, X> f, Validation<A, X> arg)
        {
            return f.Zip(arg).Map(p => p.Fst(p.Snd));
        }

        public static Validation<B, X> Ap<A, B, X>(this Func<A, B> f, Validation<A, X> arg)
        {
            return arg.Map(f);
        }
    }

    public static class PromiseApplicative
    {
        public static Promise<T> PurePromise<T>(this T value)
        {
            var p = new Promise<T>();
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
        public static Future<T> PureFuture<T>(this T value)
        {
            return new Future<T>(cb => cb(value), Strategies.Id);
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

    public static class EnumerableApplicative
    {
        public static IEnumerable<T> PureEnumerable<T>(this T value)
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
