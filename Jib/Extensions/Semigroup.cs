namespace Jib.Extensions
{
    public static class MaybeSemigroup
    {
        public static Maybe<A> SemiOp<A>(this Maybe<A> maybe, Maybe<A> other)
        {
            return Semigroup.Maybe<A>().Op(maybe, other);
        }
    }

    public static class EitherSemigroup
    {
        public static Either<A, X> SemiOp<A, X>(this Either<A, X> either, Either<A, X> other)
        {
            return Semigroup.Either<A, X>().Op(either, other);
        }
    }
}
