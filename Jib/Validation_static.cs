using System;
using System.Collections.Generic;
using System.Linq;
using Jib.Syntax;

namespace Jib
{
    public static class Validation
    {
        public static Validation<X, A> Success<X, A>(A successValue)
        {
            return new Validation<X, A>(successValue);
        }

        public static Validation<X, A> Failure<X, A>(X failureValue)
        {
            return new Validation<X, A>(NonEmptyLazyList.Single(failureValue));
        }

        public static Validation<X, A> Failure<X, A>(NonEmptyLazyList<X> failures)
        {
            return new Validation<X, A>(failures);
        }

        public static Validation<X, A> SuccessIf<X, A>(bool test, Func<A> success, Func<X> failure)
        {
            return test ? Success<X, A>(success()) : Failure<X, A>(failure());
        }

        public static Validation<X, A> SuccessIf<X, A>(bool test, A success, X failure)
        {
            return test ? Success<X, A>(success) : Failure<X, A>(failure);
        }

        public static Validation<X, A> FailureIf<X, A>(bool test, Func<A> success, Func<X> failure)
        {
            return test ? Failure<X, A>(failure()) : Success<X, A>(success());
        }

        public static Validation<X, A> FailureIf<X, A>(bool test, A success, X failure)
        {
            return test ? Failure<X, A>(failure) : Success<X, A>(success);
        }

        public static A SuccessOr<X, A>(this Validation<X, A> validation, Func<A> orElse)
        {
            return validation.Cata(xs => orElse(), a => a);
        }

        public static Z Cata1<A, X, Z>(this Validation<X, A> validation, Func<A, Z> success, Func<X, Z> failure)
        {
            return validation.Cata(xs => failure(xs.Head), success);
        }

        public static bool IsSuccess<X, A>(this Validation<X, A> validation)
        {
            return validation.Cata(x => false, a => true);
        }

        public static bool IsFailure<X, A>(this Validation<X, A> validation)
        {
            return validation.Cata(x => true, a => false);
        }

        public static IEnumerable<A> Successes<X, A>(this IEnumerable<Validation<X, A>> enumerable)
        {
            return enumerable.SelectMany(v => v.SuccessEnumerable());
        }

        public static IEnumerable<X> Failures<X, A>(this IEnumerable<Validation<X, A>> enumerable)
        {
            return enumerable.SelectMany(v => v.FailureEnumerable());
        }
    }
}