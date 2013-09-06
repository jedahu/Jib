namespace Jib
{
    public static class Semigroup
    {
        public static readonly ISemigroup<int> IntSum = Monoid.IntSum;
        public static readonly ISemigroup<int> IntProduct = Monoid.IntProduct;

        public static ISemigroup<Maybe<A>> Maybe<A>()
        {
            return new MaybeSemigroup<A>();
        }

        public static ISemigroup<Either<A, X>> Either<A, X>()
        {
            return new EitherSemigroup<A, X>();
        }

        private class MaybeSemigroup<A>
            : ISemigroup<Maybe<A>>
        {
            public Maybe<A> Op(Maybe<A> t1, Maybe<A> t2)
            {
                return t1.Cata(a => t1, () => t2);
            }
        }

        private class EitherSemigroup<A, X>
            : ISemigroup<Either<A, X>>
        {
            public Either<A, X> Op(Either<A, X> t1, Either<A, X> t2)
            {
                return t1.Cata(a => t1, x => t2);
            }
        }
    }
}
