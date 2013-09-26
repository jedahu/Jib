using System;

namespace Jib.Impure
{
    public static class MaybeSideEffects
    {
        public static void CataVoid<A>(this Maybe<A> maybe, Action nothing, Action<A> just)
        {
            maybe.Cata(Unit.Func(nothing), Unit.Func(just));
        }

        public static void JustRun<A>(this Maybe<A> maybe, Action<A> action)
        {
            maybe.CataVoid(() => { }, action);
        }

        public static void NothingRun<A>(this Maybe<A> maybe, Action action)
        {
            maybe.CataVoid(action, a => { });
        }

        public static void JustRunWhen<A>(this Maybe<A> maybe, Func<A, bool> predicate, Action<A> action)
        {
            maybe.JustRun(a => { if (predicate(a)) action(a); });
        }
    }

    public static class EitherSideEffects
    {
        public static void CataVoid<X, A>(this Either<X, A> either, Action<X> left, Action<A> right)
        {
            either.Cata(Unit.Func(left), Unit.Func(right));
        }

        public static void LeftRun<X, A>(this Either<X, A> either, Action<X> action)
        {
            either.CataVoid(action, x => { });
        }

        public static void RightRun<X, A>(this Either<X, A> either, Action<A> action)
        {
            either.CataVoid(a => { }, action);
        }

        public static void LeftRunWhen<X, A>(this Either<X, A> either, Func<X, bool> predicate, Action<X> action)
        {
            either.LeftRun(a => { if (predicate(a)) action(a); });
        }

        public static void RightRunWhen<X, A>(this Either<X, A> either, Func<A, bool> predicate, Action<A> action)
        {
            either.RightRun(x => { if (predicate(x)) action(x); });
        }
    }

    public static class ValidationSideEffects
    {
        public static void CataVoid<X, A>(this Validation<X, A> validation, Action<NonEmptyLazyList<X>> failure, Action<A> success)
        {
            validation.Cata(Unit.Func(failure), Unit.Func(success));
        }

        public static void CataVoid1<X, A>(this Validation<X, A> validation, Action<X> failure, Action<A> success)
        {
            validation.Cata1(Unit.Func(success), Unit.Func(failure));
        }

        public static void SuccessRun<X, A>(this Validation<X, A> validation, Action<A> success)
        {
            validation.CataVoid(xs => { }, success);
        }

        public static void FailureRun<X, A>(this Validation<X, A> validation, Action<NonEmptyLazyList<X>> failure)
        {
            validation.CataVoid(failure, a => { });
        }

        public static void SuccessRunWhen<X, A>(this Validation<X, A> validation, Func<A, bool> test, Action<A> success)
        {
            validation.CataVoid(xs => { }, a => { if (test(a)) success(a); });
        }

        public static void FailureRunWhen<X, A>(this Validation<X, A> validation, Func<NonEmptyLazyList<X>, bool> test, Action<NonEmptyLazyList<X>> failure)
        {
            validation.CataVoid(xs => { if (test(xs)) failure(xs); }, a => { });
        }

        public static void Failure1Run<X, A>(this Validation<X, A> validation, Action<X> failure)
        {
            validation.FailureRun(xs => failure(xs.Head));
        }

        public static void Failure1RunWhen<X, A>(this Validation<X, A> validation, Func<X, bool> test, Action<X> failure)
        {
            validation.FailureRunWhen(xs => test(xs.Head), xs => failure(xs.Head));
        }
    }
}
