using System;

namespace Jib.Extensions
{
    public static class MaybeSideEffects
    {
        public static void CataVoid<A>(this Maybe<A> maybe, Action<A> just, Action nothing)
        {
            maybe.Cata(Unit.Func(just), Unit.Func(nothing));
        }

        public static void JustRun<A>(this Maybe<A> maybe, Action<A> action)
        {
            maybe.CataVoid(action, () => { });
        }

        public static void NothingRun<A>(this Maybe<A> maybe, Action action)
        {
            maybe.CataVoid(a => { }, action);
        }

        public static void JustRunWhen<A>(this Maybe<A> maybe, Func<A, bool> predicate, Action<A> action)
        {
            maybe.JustRun(a => { if (predicate(a)) action(a); });
        }
    }

    public static class EitherSideEffects
    {
        public static void CataVoid<A, X>(this Either<A, X> either, Action<A> left, Action<X> right)
        {
            either.Cata(Unit.Func(left), Unit.Func(right));
        }

        public static void LeftRun<A, X>(this Either<A, X> either, Action<A> action)
        {
            either.CataVoid(action, x => { });
        }

        public static void RightRun<A, X>(this Either<A, X> either, Action<X> action)
        {
            either.CataVoid(a => { }, action);
        }

        public static void LeftRunWhen<A, X>(this Either<A, X> either, Func<A, bool> predicate, Action<A> action)
        {
            either.LeftRun(a => { if (predicate(a)) action(a); });
        }

        public static void RightRunWhen<A, X>(this Either<A, X> either, Func<X, bool> predicate, Action<X> action)
        {
            either.RightRun(x => { if (predicate(x)) action(x); });
        }
    }

    public static class ValidationSideEffects
    {
        public static void CataVoid<A, X>(this Validation<A, X> validation, Action<A> success, Action<NonEmptyLazyList<X>> failure)
        {
            validation.Cata(Unit.Func(success), Unit.Func(failure));
        }

        public static void CataVoid1<A, X>(this Validation<A, X> validation, Action<A> success, Action<X> failure)
        {
            validation.Cata1(Unit.Func(success), Unit.Func(failure));
        }

        public static void SuccessRun<A, X>(this Validation<A, X> validation, Action<A> success)
        {
            validation.CataVoid(success, xs => { });
        }

        public static void FailureRun<A, X>(this Validation<A, X> validation, Action<NonEmptyLazyList<X>> failure)
        {
            validation.CataVoid(a => { }, failure);
        }

        public static void SuccessRunWhen<A, X>(this Validation<A, X> validation, Func<A, bool> test, Action<A> success)
        {
            validation.CataVoid(a => { if (test(a)) success(a); }, xs => { });
        }

        public static void FailureRunWhen<A, X>(this Validation<A, X> validation, Func<NonEmptyLazyList<X>, bool> test, Action<NonEmptyLazyList<X>> failure)
        {
            validation.CataVoid(a => { }, xs => { if (test(xs)) failure(xs); });
        }

        public static void Failure1Run<A, X>(this Validation<A, X> validation, Action<X> failure)
        {
            validation.FailureRun(xs => failure(xs.Head));
        }

        public static void Failure1RunWhen<A, X>(this Validation<A, X> validation, Func<X, bool> test, Action<X> failure)
        {
            validation.FailureRunWhen(xs => test(xs.Head), xs => failure(xs.Head));
        }
    }
}
