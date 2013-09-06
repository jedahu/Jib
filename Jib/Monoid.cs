namespace Jib
{
    public static class Monoid
    {
        public static readonly IMonoid<int> IntSum = new IntSumMonoid();

        public static readonly IMonoid<int> IntProduct = new IntProductMonoid();

        public static IMonoid<Maybe<A>> Maybe<A>()
        {
            return new MaybeMonoid<A>();
        }

        private class IntSumMonoid : IMonoid<int>
        {
            public int Op(int t1, int t2) { return t1 + t2; }
            public int Zero { get { return 0; } }
        }

        private class IntProductMonoid : IMonoid<int>
        {
            public int Op(int t1, int t2) { return t1*t2; }
            public int Zero { get { return 1; } }
        }

        private class MaybeMonoid<A>
            : IMonoid<Maybe<A>>
        {
            private ISemigroup<Maybe<A>> semi = Semigroup.Maybe<A>();

            public Maybe<A> Op(Maybe<A> t1, Maybe<A> t2)
            {
                return semi.Op(t1, t2);
            }

            public Maybe<A> Zero
            {
                get { return Jib.Maybe.Nothing<A>(); }
            }
        }
    }
}