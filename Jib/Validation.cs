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
}