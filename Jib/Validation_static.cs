using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib
{
    public static class Validation
    {
        #region Constructors

        public static Validation<TA, TX> Success<TA, TX>(TA successValue)
        {
            return new Validation<TA, TX>(successValue);
        }

        public static Validation<TA, TX> Failure<TA, TX>(TX failureValue)
        {
            return new Validation<TA, TX>(NonEmptyLazyList.Single(failureValue));
        }

        public static Validation<TA, TX> Failure<TA, TX>(NonEmptyLazyList<TX> failures)
        {
            return new Validation<TA, TX>(failures);
        }

        public static Validation<TA, TX> SuccessIf<TA, TX>(bool test, Func<TA> success, Func<TX> failure)
        {
            return test ? Success<TA, TX>(success()) : Failure<TA, TX>(failure());
        }

        public static Validation<TA, TX> SuccessIf<TA, TX>(bool test, TA success, TX failure)
        {
            return test ? Success<TA, TX>(success) : Failure<TA, TX>(failure);
        }

        public static Validation<TA, TX> FailureIf<TA, TX>(bool test, Func<TA> success, Func<TX> failure)
        {
            return test ? Failure<TA, TX>(failure()) : Success<TA, TX>(success());
        }

        public static Validation<TA, TX> FailureIf<TA, TX>(bool test, TA success, TX failure)
        {
            return test ? Failure<TA, TX>(failure) : Success<TA, TX>(success);
        }

        #endregion

        #region Enumerable operations

        public static IEnumerable<TA> Successes<TA, TX>(this IEnumerable<Validation<TA, TX>> enumerable)
        {
            return enumerable.SelectMany(v => v.SuccessEnumerable);
        }

        public static IEnumerable<TX> Failures<TA, TX>(this IEnumerable<Validation<TA, TX>> enumerable)
        {
            return enumerable.SelectMany(v => v.FailureEnumerable);
        }

        #endregion

        #region Combining Validations into tuples
        /// Useful with Arity.Uncurry().
        /// Instead of this:
        ///     var result1 = form a in va from b in vb select func(a, b);
        /// 
        /// you can write this:
        ///     var result2 = Validation.Combine(va, vb).Map(func.Uncurry());
        /// 
        /// In actual fact the above two expressions are not equivalent.
        /// If both va and vb are failures, result1 will contain only the failure from va,
        /// result2 will contain the concatenation of both va and vb's failures.

        /// <summary>
        /// Combine two Validations into a Validation{Tuple{,}}, concatenating failures.
        /// </summary>
        public static Validation<Tuple<TA, TB>, TX> Combine<TA, TB, TX>(
            Validation<TA, TX> v1,
            Validation<TB, TX> v2)
        {
            var success = from s1 in v1
                          from s2 in v2
                          select Tuple.Create(s1, s2);
            var failure =
                v1.Map<object>(a => a) &&
                v2.Map<object>(a => a);
            return failure.Bind(a => success);
        }

        /// <summary>
        /// Combine three Validations into a Validation{Tuple{,,}, concatenating failures.
        /// </summary>
        public static Validation<Tuple<TA, TB, TC>, TX> Combine<TA, TB, TC, TX>(
            Validation<TA, TX> v1,
            Validation<TB, TX> v2,
            Validation<TC, TX> v3)
        {
            var success = from s1 in v1
                          from s2 in v2
                          from s3 in v3
                          select Tuple.Create(s1, s2, s3);
            var failure =
                v1.Map<object>(a => a) &&
                v2.Map<object>(a => a) &&
                v3.Map<object>(a => a);
            return failure.Bind(a => success);
        }

        /// <summary>
        /// Combine four Validations into a Validation{Tuple{,,,}, concatenating failures.
        /// </summary>
        public static Validation<Tuple<TA, TB, TC, TD>, TX> Combine<TA, TB, TC, TD, TX>(
            Validation<TA, TX> v1,
            Validation<TB, TX> v2,
            Validation<TC, TX> v3,
            Validation<TD, TX> v4)
        {
            var success = from s1 in v1
                          from s2 in v2
                          from s3 in v3
                          from s4 in v4
                          select Tuple.Create(s1, s2, s3, s4);
            var failure =
                v1.Map<object>(a => a) &&
                v2.Map<object>(a => a) &&
                v3.Map<object>(a => a) &&
                v4.Map<object>(a => a);
            return failure.Bind(a => success);
        }

        /// <summary>
        /// Combine five Validations into a Validation{Tuple{,,,,}, concatenating failures.
        /// </summary>
        public static Validation<Tuple<TA, TB, TC, TD, TE>, TX> Combine<TA, TB, TC, TD, TE, TX>(
            Validation<TA, TX> v1,
            Validation<TB, TX> v2,
            Validation<TC, TX> v3,
            Validation<TD, TX> v4,
            Validation<TE, TX> v5)
        {
            var success = from s1 in v1
                          from s2 in v2
                          from s3 in v3
                          from s4 in v4
                          from s5 in v5
                          select Tuple.Create(s1, s2, s3, s4, s5);
            var failure =
                v1.Map<object>(a => a) &&
                v2.Map<object>(a => a) &&
                v3.Map<object>(a => a) &&
                v4.Map<object>(a => a) &&
                v5.Map<object>(a => a);
            return failure.Bind(a => success);
        }

        #endregion

        #region Applying functions to Validation values

        public static Validation<TR, TX> Apply<TA, TR, TX>(this Func<TA, TR> func, Validation<TA, TX> arg)
        {
            return arg.Map(func);
        }

        public static Validation<TR, TX> Apply<TA, TB, TR, TX>(this Func<TA, TB, TR> func, Validation<TA, TX> arg1, Validation<TB, TX> arg2)
        {
            return Combine(arg1, arg2).Map(func.Tuplize());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TR, TX>(
            this Func<TA, TB, TC, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3)
        {
            return Combine(arg1, arg2, arg3).Map(func.Tuplize());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TD, TR, TX>(
            this Func<TA, TB, TC, TD, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3,
            Validation<TD, TX> arg4)
        {
            return Combine(arg1, arg2, arg3, arg4).Map(func.Tuplize());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TD, TE, TR, TX>(
            this Func<TA, TB, TC, TD, TE, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3,
            Validation<TD, TX> arg4,
            Validation<TE, TX> arg5)
        {
            return Combine(arg1, arg2, arg3, arg4, arg5).Map(func.Tuplize());
        }

        #endregion

        #region Methods for interoperating with non-nullsafe code

        public static Validation<TA, TX> ToValidation<TA, TX>(this TA value, TX elseValue) where TA : class
        {
            return value == null ? Failure<TA, TX>(elseValue) : Success<TA, TX>(value);
        }

        public static Validation<TA, TX> ToValidation<TA, TX>(this TA value, Func<TX> elseFunc) where TA : class
        {
            return value == null ? Failure<TA, TX>(elseFunc()) : Success<TA, TX>(value);
        }

        public static Validation<TA, TX> ToValidation<TA, TX>(this TA? value, TX elseValue) where TA : struct
        {
            return value.HasValue ? Success<TA, TX>(value.Value) : Failure<TA, TX>(elseValue);
        }

        public static Validation<TA, TX> ToValidation<TA, TX>(this TA? value, Func<TX> elseFunc) where TA : struct
        {
            return value.HasValue ? Success<TA, TX>(value.Value) : Failure<TA, TX>(elseFunc());
        }

        #endregion

        #region Linq methods

        public static Validation<TB, TX> Select<TA, TB, TX>(this Validation<TA, TX> validation, Func<TA, TB> selector)
        {
            return validation.Map(selector);
        }

        public static Validation<TB, TX> SelectMany<TA, TB, TX>(this Validation<TA, TX> validation, Func<TA, Validation<TB, TX>> selector)
        {
            return validation.Bind(selector);
        }

        public static Validation<TC, TX> SelectMany<TA, TB, TC, TX>(this Validation<TA, TX> validation, Func<TA, Validation<TB, TX>> selector, Func<TA, TB, TC> combineFunc)
        {
            return validation.Bind(a => selector(a).Bind(b => Success<TC, TX>(combineFunc(a, b))));
        }

        #endregion
    }
}