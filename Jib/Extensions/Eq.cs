namespace Jib.Extensions
{
    public static class MaybeEq
    {
        public static bool Eq<A>(this Maybe<A> maybe, Maybe<A> other, IEq<A> tEq)
        {
            return Jib.Eq.Maybe(tEq).Eq(maybe, other);
        }
    }

    public static class EitherEq
    {
        public static bool Eq<A, X>(this Either<A, X> either, Either<A, X> other, IEq<A> tEq, IEq<X> xEq)
        {
            return Jib.Eq.Either(tEq, xEq).Eq(either, other);
        }
    }
}
