using System;
using System.Collections.Generic;
using System.Linq;
using Jib.Extensions;

namespace Jib
{
    public static class Validation
    {
        public static Validation<A, X> Success<A, X>(A successValue)
        {
            return new Validation<A, X>(successValue);
        }

        public static Validation<A, X> Failure<A, X>(X failureValue)
        {
            return new Validation<A, X>(NonEmptyLazyList.Single(failureValue));
        }

        public static Validation<A, X> Failure<A, X>(NonEmptyLazyList<X> failures)
        {
            return new Validation<A, X>(failures);
        }

        public static Validation<A, X> SuccessIf<A, X>(bool test, Func<A> success, Func<X> failure)
        {
            return test ? Success<A, X>(success()) : Failure<A, X>(failure());
        }

        public static Validation<A, X> SuccessIf<A, X>(bool test, A success, X failure)
        {
            return test ? Success<A, X>(success) : Failure<A, X>(failure);
        }

        public static Validation<A, X> FailureIf<A, X>(bool test, Func<A> success, Func<X> failure)
        {
            return test ? Failure<A, X>(failure()) : Success<A, X>(success());
        }

        public static Validation<A, X> FailureIf<A, X>(bool test, A success, X failure)
        {
            return test ? Failure<A, X>(failure) : Success<A, X>(success);
        }

        public static Z Cata1<A, X, Z>(this Validation<A, X> validation, Func<A, Z> success, Func<X, Z> failure)
        {
            return validation.Cata(success, xs => failure(xs.Head));
        }

        public static bool IsSuccess<A, X>(this Validation<A, X> validation)
        {
            return validation.Cata(a => true, x => false);
        }

        public static bool IsFailure<A, X>(this Validation<A, X> validation)
        {
            return validation.Cata(a => false, x => true);
        }

        public static IEnumerable<A> Successes<A, X>(this IEnumerable<Validation<A, X>> enumerable)
        {
            return enumerable.SelectMany(v => v.SuccessEnumerable());
        }

        public static IEnumerable<X> Failures<A, X>(this IEnumerable<Validation<A, X>> enumerable)
        {
            return enumerable.SelectMany(v => v.FailureEnumerable());
        }
    }
}