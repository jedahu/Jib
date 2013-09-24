using Jib.Syntax;

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

        public static ISemigroup<Either<X, A>> Either<X, A>()
        {
            return new EitherSemigroup<X, A>();
        }

        public static ISemigroup<Validation<X, A>> ValidationSuccess<X, A>()
        {
            return new ValidationSuccessSemigroup<X, A>();
        }

        public static ISemigroup<Validation<X, A>> ValidationFailure<X, A>()
        {
            return new ValidationFailureSemigroup<X, A>();
        }

        public static ISemigroup<NonEmptyLazyList<A>> NonEmptyLazyList<A>()
        {
            return new NonEmptyLazyListSemigroup<A>();
        }

        private class MaybeSemigroup<A>
            : ISemigroup<Maybe<A>>
        {
            public Maybe<A> Op(Maybe<A> t1, Maybe<A> t2)
            {
                return t1.Cata(() => t2, a => t1);
            }
        }

        private class EitherSemigroup<X, A>
            : ISemigroup<Either<X, A>>
        {
            public Either<X, A> Op(Either<X, A> t1, Either<X, A> t2)
            {
                return t1.Cata(x => t2, a => t1);
            }
        }

        private class ValidationSuccessSemigroup<X, A>
            : ISemigroup<Validation<X, A>>
        {
            public Validation<X, A> Op(Validation<X, A> t1, Validation<X, A> t2)
            {
                return t1.Cata(xs1 => t2.Cata(xs2 => Validation.Failure<X, A>(xs1.SemiOp(xs2)), a2 => t2), a1 => t1);
            }
        }

        private class ValidationFailureSemigroup<X, A>
            : ISemigroup<Validation<X, A>>
        {
            public Validation<X, A> Op(Validation<X, A> t1, Validation<X, A> t2)
            {
                return t1.Cata(xs1 => t2.Cata(xs2 => Validation.Failure<X, A>(xs1.SemiOp(xs2)), a2 => t1), a1 => t2);
            }
        }

        private class NonEmptyLazyListSemigroup<A>
            : ISemigroup<NonEmptyLazyList<A>>
        {
            public NonEmptyLazyList<A> Op(NonEmptyLazyList<A> t1, NonEmptyLazyList<A> t2)
            {
                return new NonEmptyLazyList<A>(
                    t1.Head,
                    () => Jib.Maybe.Just(t1.Tail.Cata(() => t2, nel => Op(nel, t2))));
            }
        }
    }
}
