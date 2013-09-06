using System;

namespace Jib.Extensions
{
    public static class MaybeSideEffects
    {
        public static void CataVoid<A>(this Maybe<A> maybe, Action<A> just, Action nothing)
        {
            maybe.Cata(a => { just(a); return 0; }, () => { nothing(); return 0; });
        }

        public static void JustAction<A>(this Maybe<A> maybe, Action<A> action)
        {
            maybe.CataVoid(action, () => { });
        }

        public static void NothingAction<A>(this Maybe<A> maybe, Action action)
        {
            maybe.CataVoid(a => { }, action);
        }

        public static void JustActionWhen<A>(this Maybe<A> maybe, Func<A, bool> predicate, Action<A> action)
        {
            maybe.JustAction(a => { if (predicate(a)) action(a); });
        }
    }

    public static class EitherSideEffects
    {
        public static void CataVoid<A, X>(this Either<A, X> either, Action<A> left, Action<X> right)
        {
            either.Cata(a => { left(a); return 0; }, x => { right(x); return 0; });
        }

        public static void LeftAction<A, X>(this Either<A, X> either, Action<A> action)
        {
            either.CataVoid(action, x => { });
        }

        public static void RightAction<A, X>(this Either<A, X> either, Action<X> action)
        {
            either.CataVoid(a => { }, action);
        }

        public static void LeftActionWhen<A, X>(this Either<A, X> either, Func<A, bool> predicate, Action<A> action)
        {
            either.LeftAction(a => { if (predicate(a)) action(a); });
        }

        public static void RightActionWhen<A, X>(this Either<A, X> either, Func<X, bool> predicate, Action<X> action)
        {
            either.RightAction(x => { if (predicate(x)) action(x); });
        }
    }
}
