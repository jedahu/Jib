using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Extensions
{
    public static class MaybeMorphisms
    {
        public static Validation<A, X> Validation<A, X>(this Maybe<A> maybe, Func<X> fail)
        {
            return maybe.Cata(
                Jib.Validation.Success<A, X>,
                () => Jib.Validation.Failure<A, X>(fail()));
        }

        public static Either<A, X> Either<A, X>(this Maybe<A> maybe, Func<X> right)
        {
            return maybe.Cata(
                Jib.Either.Left<A, X>,
                () => Jib.Either.Right<A, X>((right())));
        }
    }

    public static class EitherMorphisms
    {
        public static Validation<A, X> Validation<A, X>(this Either<A, X> either)
        {
            return either.Cata(
                Jib.Validation.Success<A, X>,
                Jib.Validation.Failure<A, X>);
        }

        public static Maybe<A> LeftMaybe<A, X>(this Either<A, X> either)
        {
            return either.Cata(
                Maybe.Just,
                x => Maybe.Nothing<A>());
        }

        public static Maybe<X> RightMaybe<A, X>(this Either<A, X> either)
        {
            return either.Cata(
                a => Maybe.Nothing<X>(),
                Maybe.Just);
        }

        public static IEnumerable<A> LeftEnumerable<A, X>(this Either<A, X> either)
        {
            return either.Cata(
                a => a.PureEnumerable(),
                x => Enumerable.Empty<A>());
        }

        public static IEnumerable<X> RightEnumerable<A, X>(this Either<A, X> either)
        {
            return either.Cata(
                a => Enumerable.Empty<X>(),
                x => x.PureEnumerable());
        }
    }

    public static class ValidationMorphisms
    {
        public static IEnumerable<A> SuccessEnumerable<A, X>(this Validation<A, X> validation)
        {
            return validation.Cata(a => a.PureEnumerable(), xs => Enumerable.Empty<A>());
        }

        public static IEnumerable<X> FailureEnumerable<A, X>(this Validation<A, X> validation)
        {
            return validation.Cata(a => Enumerable.Empty<X>(), xs => xs.Enumerable());
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
