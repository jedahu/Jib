using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib
{
    public static class Either
    {
        #region Constructors

        /// <summary>
        /// Construct a Left Either from `leftValue`.
        /// </summary>
        public static Either<TA, TX> Left<TA, TX>(TA leftValue)
        {
            return new Either<TA, TX>(leftValue, default(TX), true);
        }

        /// <summary>
        /// Construct a Right Either from `rightValue`.
        /// </summary>
        public static Either<TA, TX> Right<TA, TX>(TX rightValue)
        {
            return new Either<TA, TX>(default(TA), rightValue, false);
        }

        /// <summary>
        /// Construct a Left Either from `left` if `isLeft` is true. Otherwise construct
        /// a Right Either from `right`.
        /// </summary>
        public static Either<TA, TX> LeftIf<TA, TX>(bool isLeft, Func<TA> left, Func<TX> right)
        {
            return isLeft ? Left<TA, TX>(left()) : Right<TA, TX>(right());
        }

        /// <summary>
        /// Construct a Right Either from `right` if `isRight` is true. Otherwise construct
        /// a Left Either from `left`.
        /// </summary>
        public static Either<TA, TX> RightIf<TA, TX>(bool isRight, Func<TA> left, Func<TX> right)
        {
            return isRight ? Right<TA, TX>(right()) : Left<TA, TX>(left());
        }

        /// <summary>
        /// Construct a Left Either from `left` if `isLeft` is true. Otherwise construct
        /// a Right Either from `right`.
        /// </summary>
        public static Either<TA, TX> LeftIf<TA, TX>(bool isLeft, TA left, TX right)
        {
            return isLeft ? Left<TA, TX>(left) : Right<TA, TX>(right);
        }

        /// <summary>
        /// Construct a Right Either from `right` if `isRight` is true. Otherwise construct
        /// a Left Either from `left`.
        /// </summary>
        public static Either<TA, TX> RightIf<TA, TX>(bool isRight, TA left, TX right)
        {
            return isRight ? Right<TA, TX>(right) : Left<TA, TX>(left);
        }

        #endregion

        /// <summary>
        /// Convert a possibly null value to an Either.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="elseValue">A value to use as the Either's Right if 'value' is null.</param>
        public static Either<TA, TX> ToEither<TA, TX>(this TA value, TX elseValue) where TA : class
        {
            return value == null ? Right<TA, TX>(elseValue) : Left<TA, TX>(value);
        }

        /// <summary>
        /// Convert a possibly null value to an Either.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="elseFunc">An function to call to supply a right value if 'value' is null.</param>
        public static Either<TA, TX> ToEither<TA, TX>(this TA value, Func<TX> elseFunc) where TA : class
        {
            return value == null ? Right<TA, TX>(elseFunc()) : Left<TA, TX>(value);
        }

        /// <summary>
        /// Convert a nullable value to an Either.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="elseValue">A value to use as the Either's Right if 'value' is null.</param>
        public static Either<TA, TX> ToEither<TA, TX>(this TA? value, TX elseValue) where TA : struct
        {
            return value.HasValue ? Left<TA, TX>(value.Value) : Right<TA, TX>(elseValue);
        }

        /// <summary>
        /// Convert a nullable value to an Either.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="elseFunc">An function to call to supply a right value if 'value' is null.</param>
        public static Either<TA, TX> ToEither<TA, TX>(this TA? value, Func<TX> elseFunc) where TA : struct
        {
            return value.HasValue ? Left<TA, TX>(value.Value) : Right<TA, TX>(elseFunc());
        }

        /// <summary>
        /// Get a value from a dictionary wrapped in a Either. If the value is not in the dictionary a right
        /// Either will be returned containing the supplied elseValue.
        /// </summary>
        public static Either<TA, TX> EitherGet<TK, TA, TX>(this IDictionary<TK, TA> dict, TK key, TX elseValue)
        {
            try
            {
                return Left<TA, TX>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Right<TA, TX>(elseValue);
            }
        }

        /// <summary>
        /// Get a value from a dictionary wrapped in a Either. If the value is not in the dictionary a right
        /// Either will be returned containing the return value of the supplied elseFunc.
        /// </summary>
        public static Either<TA, TX> EitherGet<TK, TA, TX>(this IDictionary<TK, TA> dict, TK key, Func<TX> elseFunc)
        {
            try
            {
                return Left<TA, TX>(dict[key]);
            }
            catch (KeyNotFoundException)
            {
                return Right<TA, TX>(elseFunc());
            }
        }

        /// <summary>
        /// Take an enumerable of Either{TA, TX}and return an enumerable of TA.
        /// </summary>
        public static IEnumerable<TA> Lefts<TA, TX>(this IEnumerable<Either<TA, TX>> eithers)
        {
            return eithers.SelectMany(e => e.LeftEnumerable);
        }

        /// <summary>
        /// Take an enumerable of Either{TA, TX}and return an enumerable of TX.
        /// </summary>
        public static IEnumerable<TX> Rights<TA, TX>(this IEnumerable<Either<TA, TX>> eithers)
        {
            return eithers.SelectMany(e => e.RightEnumerable);
        }

        public static Either<TB, TX> Select<TA, TB, TX>(this Either<TA, TX> either, Func<TA, TB> selectFunc)
        {
            return either.Map(selectFunc);
        }

        public static Either<TB, TX> SelectMany<TA, TB, TX>(this Either<TA, TX> either, Func<TA, Either<TB, TX>> selectManyFunc)
        {
            return either.Bind(selectManyFunc);
        }

        public static Either<TC, TX> SelectMany<TA, TB, TC, TX>(this Either<TA, TX> either, Func<TA, Either<TB, TX>> selectManyFunc, Func<TA, TB, TC> combineFunc)
        {
            return either.Bind(a => selectManyFunc(a).Bind(b => Left<TC, TX>(combineFunc(a, b))));
        }
    }
}