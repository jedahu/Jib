namespace Jib.Syntax
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
        public static Either<X, A> SemiOp<X, A>(this Either<X, A> either, Either<X, A> other)
        {
            return Semigroup.Either<X, A>().Op(either, other);
        }
    }

    public static class NonEmptyLazyListSemigroup
    {
        public static NonEmptyLazyList<A> SemiOp<A>(this NonEmptyLazyList<A> nel, NonEmptyLazyList<A> other)
        {
            return Semigroup.NonEmptyLazyList<A>().Op(nel, other);
        }
    }
}
