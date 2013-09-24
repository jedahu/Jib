using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Syntax
{
    public static class MaybeMorphisms
    {
        public static Validation<X, A> Validation<X, A>(this Maybe<A> maybe, Func<X> fail)
        {
            return maybe.Cata(() => Jib.Validation.Failure<X, A>(fail()), Jib.Validation.Success<X, A>);
        }

        public static Either<X, A> Either<X, A>(this Maybe<A> maybe, Func<X> left)
        {
            return maybe.Cata(() => Jib.Either.Left<X, A>((left())), Jib.Either.Right<X, A>);
        }
    }

    public static class EitherMorphisms
    {
        public static Validation<X, A> Validation<X, A>(this Either<X, A> either)
        {
            return either.Cata(
                Jib.Validation.Failure<X, A>,
                Jib.Validation.Success<X, A>);
        }

        public static Maybe<A> RightMaybe<X, A>(this Either<X, A> either)
        {
            return either.Cata(
                x => Maybe.Nothing<A>(),
                Maybe.Just);
        }

        public static Maybe<X> LeftMaybe<X, A>(this Either<X, A> either)
        {
            return either.Cata(
                Maybe.Just,
                a => Maybe.Nothing<X>());
        }

        public static IEnumerable<A> RightEnumerable<X, A>(this Either<X, A> either)
        {
            return either.Cata(
                x => Enumerable.Empty<A>(),
                a => a.PureEnumerable());
        }

        public static IEnumerable<X> LeftEnumerable<X, A>(this Either<X, A> either)
        {
            return either.Cata(
                x => x.PureEnumerable(),
                a => Enumerable.Empty<X>());
        }
    }

    public static class ValidationMorphisms
    {
        public static IEnumerable<A> SuccessEnumerable<X, A>(this Validation<X, A> validation)
        {
            return validation.Cata(xs => Enumerable.Empty<A>(), a => a.PureEnumerable());
        }

        public static IEnumerable<X> FailureEnumerable<X, A>(this Validation<X, A> validation)
        {
            return validation.Cata(xs => xs.Enumerable(), a => Enumerable.Empty<X>());
        }
    }

    public static class EnumerableMorphisms
    {
        public static Maybe<NonEmptyLazyList<A>> NonEmptyLazyList<A>(this IEnumerable<A> enumerable)
        {
            return enumerable.Uncons().Map(Jib.NonEmptyLazyList.Create);
        }
        
        public static LazyList<A> LazyList<A>(this IEnumerable<A> enumerable)
        {
            return Jib.LazyList.Create(enumerable);
        }
    }
}
