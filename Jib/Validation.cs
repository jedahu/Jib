using System;

namespace Jib
{
    /// <summary>
    /// A data type that encodes the concept of failure.
    /// </summary>
    public struct Validation<A, X>
    {
        private readonly A successValue;
        private readonly NonEmptyLazyList<X> failuresValue;
        private readonly bool isSuccess;

        public Validation(A successValue)
        {
            this.successValue = successValue;
            failuresValue = null;
            isSuccess = true;
        }

        public Validation(NonEmptyLazyList<X> failures)
        {
            successValue = default(A);
            failuresValue = failures;
            isSuccess = false;
        }

        public Z Cata<Z>(Func<A, Z> success, Func<NonEmptyLazyList<X>, Z> failure)
        {
            return isSuccess ? success(successValue) : failure(SafeFailures);
        }

        private NonEmptyLazyList<X> SafeFailures
        {
            get { return failuresValue ?? NonEmptyLazyList.Single(default(X)); }
        }

        public override bool Equals(object obj)
        {
            return Unwrinkle.Equals();
        }

        public override int GetHashCode()
        {
            return Unwrinkle.GetHashCode();
        }

        public override string ToString()
        {
            return Unwrinkle.ToString();
        }
    }
}