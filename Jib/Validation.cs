using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib
{
    /// <summary>
    /// A data type that encodes the concept of failure.
    /// </summary>
    public struct Validation<TA, TX>
    {
        private readonly TA successValue;
        private readonly NonEmptyLazyList<TX> failuresValue;
        private readonly bool isSuccess;

        public Validation(TA successValue)
        {
            this.successValue = successValue;
            failuresValue = null;
            isSuccess = true;
        }

        public Validation(NonEmptyLazyList<TX> failures)
        {
            successValue = default(TA);
            failuresValue = failures;
            isSuccess = false;
        }

        #region Core members
        /// All other members are derived from these three.

        /// <summary>
        ///  True if this Validation is a Success.
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
        }

        /// <summary>
        /// True if this Validation is a Failure.
        /// </summary>
        public bool IsFailure
        {
            get { return !isSuccess; }
        }

        public TZ Fold<TZ>(Func<TA, TZ> success, Func<NonEmptyLazyList<TX>, TZ> failure)
        {
            return isSuccess ? success(successValue) : failure(SafeFailures);
        }

        #endregion

        #region Derived members
        ///  All these members are implemented in terms of Fold.

        #region Convenience methods

        public TZ Fold1<TZ>(Func<TA, TZ> success, Func<TX, TZ> failure)
        {
            return Fold(success, fs => failure(fs.First()));
        }

        #endregion

        #region Side-effecting methods

        public void FoldVoid(Action<TA> success, Action<NonEmptyLazyList<TX>> failure)
        {
            Fold(a => { success(a); return 0; }, fs => { failure(fs); return 0; });
        }

        public void FoldVoid1(Action<TA> success, Action<TX> failure)
        {
            FoldVoid(success, fs => failure(fs.First()));
        }

        public void SuccessAction(Action<TA> success)
        {
            FoldVoid(success, fs => { });
        }

        public void FailureAction(Action<NonEmptyLazyList<TX>> failure)
        {
            FoldVoid(a => { }, failure);
        }

        public void FailureAction1(Action<TX> failure)
        {
            FailureAction(fs => failure(fs.First()));
        }

        public void SuccessActionWhen(Func<TA, bool> test, Action<TA> success)
        {
            SuccessAction(a => { if (test(a)) success(a); });
        }

        public void FailureActionWhen(Func<NonEmptyLazyList<TX>, bool> test, Action<NonEmptyLazyList<TX>> failure)
        {
            FailureAction(fs => { if (test(fs)) failure(fs); });
        }

        public void FailureAction1When(Func<TX, bool> test, Action<TX> failure)
        {
            FailureAction(fs => { if (test(fs.First())) failure(fs.First()); });
        }

        #endregion

        #region Linq methods

        public Validation<TB, TX> Map<TB>(Func<TA, TB> mapFunc)
        {
            return Fold(a => Validation.Success<TB, TX>(mapFunc(a)), Validation.Failure<TB, TX>);
        }

        public Validation<TB, TX> Bind<TB>(Func<TA, Validation<TB, TX>> bindFunc)
        {
            return Fold(bindFunc, Validation.Failure<TB, TX>);
        }

        public Validation<TA, TY> MapFailure<TY>(Func<NonEmptyLazyList<TX>, TY> wrap)
        {
            return Fold(Validation.Success<TA, TY>, fs => Validation.Failure<TA, TY>(wrap(fs)));
        }

        #endregion

        #region Overloaded operators

        public static bool operator true(Validation<TA, TX> validation)
        {
            return validation.IsSuccess;
        }

        public static bool operator false(Validation<TA, TX> validation)
        {
            return validation.IsFailure;
        }

        public static Validation<TA, TX> operator &(Validation<TA, TX> v1, Validation<TA, TX> v2)
        {
            return v1.Fold(
                a1 => v2,
                fs1 => v2.Fold(
                    a2 => v1,
                    fs2 => Validation.Failure<TA, TX>(fs1.Concat(fs2))));
        }

        public static Validation<TA, TX> operator |(Validation<TA, TX> v1, Validation<TA, TX> v2)
        {
            return v1.Fold(a1 => v1, fs => v2);
        }

        public static explicit operator bool(Validation<TA, TX> validation)
        {
            return validation.IsSuccess;
        }

        #endregion

        #region Conversion members

        public Maybe<TA> Maybe
        {
            get { return Fold(Jib.Maybe.Just, fs => Jib.Maybe.Nothing<TA>()); }
        }

        public IEnumerable<TA> SuccessEnumerable
        {
            get { return Fold(a => new[] {a}, fs => new TA[] {}); }
        }

        public IEnumerable<TX> FailureEnumerable
        {
            get { return Fold<IEnumerable<TX>>(a => new TX[] {}, fs => fs); }
        }

        #endregion

        #endregion

        #region Private members

        private NonEmptyLazyList<TX> SafeFailures
        {
            get { return failuresValue ?? NonEmptyLazyList.Single(default(TX)); }
        }

        #endregion
    }

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
            return Combine(arg1, arg2).Map(func.Uncurry());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TR, TX>(
            this Func<TA, TB, TC, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3)
        {
            return Combine(arg1, arg2, arg3).Map(func.Uncurry());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TD, TR, TX>(
            this Func<TA, TB, TC, TD, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3,
            Validation<TD, TX> arg4)
        {
            return Combine(arg1, arg2, arg3, arg4).Map(func.Uncurry());
        }

        public static Validation<TR, TX> Apply<TA, TB, TC, TD, TE, TR, TX>(
            this Func<TA, TB, TC, TD, TE, TR> func,
            Validation<TA, TX> arg1,
            Validation<TB, TX> arg2,
            Validation<TC, TX> arg3,
            Validation<TD, TX> arg4,
            Validation<TE, TX> arg5)
        {
            return Combine(arg1, arg2, arg3, arg4, arg5).Map(func.Uncurry());
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