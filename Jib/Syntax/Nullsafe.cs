using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Syntax
{
    public static class MaybeNullsafe
    {
        public static Maybe<A> ToMaybe<A>(this A value)
            where A : class
        {
            return value == null ? Maybe.Nothing<A>() : Maybe.Just(value);
        }

        public static Maybe<A> ToMaybe<A>(this A? value)
            where A : struct
        {
            return value.HasValue ? Maybe.Just(value.Value) : Maybe.Nothing<A>();
        }

        public static Maybe<A> MaybeFirst<A>(this IEnumerable<A> list)
        {
            // Cannot test an IEnumerable for emptiness without 'using' it.
            // try-catch is the only way.
            try
            {
                return Maybe.Just(list.First());
            }
            catch (InvalidOperationException)
            {
                return Maybe.Nothing<A>();
            }
        }

        public static Maybe<A> MaybeGet<K, A>(this IDictionary<K, A> dict, K key)
        {
            try
            {
                return Maybe.Just(dict[key]);
            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("MaybeGet on a null IDictionary.", e);
            }
            catch (KeyNotFoundException)
            {
                return Maybe.Nothing<A>();
            }
        }
    }

    public static class EitherNullSafe
    {
        public static Either<X, A> ToEither<X, A>(this A value, Func<X> elseFunc)
            where A : class
        {
            return value == null
                ? Either.Left<X, A>(elseFunc())
                : Either.Right<X, A>(value);
        }
        
        public static Either<X, A> ToEither<X, A>(this A? value, Func<X> elseFunc)
            where A : struct
        {
            return value.HasValue
                ? Either.Right<X, A>(value.Value)
                : Either.Left<X, A>(elseFunc());
        }

        public static Either<X, A> EitherGet<K, A, X>(this IDictionary<K, A> dict, K key, X elseValue)
        {
            try
            {
                return Either.Right<X, A>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Either.Left<X, A>(elseValue);
            }
        }

        public static Either<X, A> EitherGet<K, A, X>(this IDictionary<K, A> dict, K key, Func<X> elseFunc)
        {
            try
            {
                return Either.Right<X, A>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Either.Left<X, A>(elseFunc());
            }
        }
    }

    public static class ValidationNullSafe
    {
        public static Validation<X, A> ToValidation<X, A>(this A value, Func<X> elseFunc)
            where A : class
        {
            return value == null
                       ? Validation.Failure<X, A>(elseFunc())
                       : Validation.Success<X, A>(value);
        }

        public static Validation<X, A> ToValidation<X, A>(this A? value, Func<X> elseFunc)
            where A : struct
        {
            return value.HasValue
                       ? Validation.Success<X, A>(value.Value)
                       : Validation.Failure<X, A>(elseFunc());
        }
    }
}
