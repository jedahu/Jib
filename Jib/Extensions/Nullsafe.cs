using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib.Extensions
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
        public static Either<A, X> ToEither<A, X>(this A value, Func<X> elseFunc)
            where A : class
        {
            return value == null
                ? Either.Right<A, X>(elseFunc())
                : Either.Left<A, X>(value);
        }
        
        public static Either<A, X> ToEither<A, X>(this A? value, Func<X> elseFunc)
            where A : struct
        {
            return value.HasValue
                ? Either.Left<A, X>(value.Value)
                : Either.Right<A, X>(elseFunc());
        }

        public static Either<A, X> EitherGet<K, A, X>(this IDictionary<K, A> dict, K key, X elseValue)
        {
            try
            {
                return Either.Left<A, X>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Either.Right<A, X>(elseValue);
            }
        }

        public static Either<A, X> EitherGet<K, A, X>(this IDictionary<K, A> dict, K key, Func<X> elseFunc)
        {
            try
            {
                return Either.Left<A, X>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Either.Right<A, X>(elseFunc());
            }
        }
    }

    public static class ValidationNullSafe
    {
        public static Validation<A, X> ToValidation<A, X>(this A value, Func<X> elseFunc)
            where A : class
        {
            return value == null
                       ? Validation.Failure<A, X>(elseFunc())
                       : Validation.Success<A, X>(value);
        }

        public static Validation<A, X> ToValidation<A, X>(this A? value, Func<X> elseFunc)
            where A : struct
        {
            return value.HasValue
                       ? Validation.Success<A, X>(value.Value)
                       : Validation.Failure<A, X>(elseFunc());
        }
    }
}
