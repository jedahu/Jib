using Jib.Extensions;

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

        public static ISemigroup<Validation<A, X>> ValidationSuccess<A, X>()
        {
            return new ValidationSuccessSemigroup<A, X>();
        }

        public static ISemigroup<Validation<A, X>> ValidationFailure<A, X>()
        {
            return new ValidationFailureSemigroup<A, X>();
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

        private class ValidationSuccessSemigroup<A, X>
            : ISemigroup<Validation<A, X>>
        {
            public Validation<A, X> Op(Validation<A, X> t1, Validation<A, X> t2)
            {
                return t1.Cata(
                    a1 => t1,
                    xs1 => t2.Cata(
                        a2 => t2,
                        xs2 => Validation.Failure<A, X>(xs1.SemiOp(xs2))));
            }
        }

        private class ValidationFailureSemigroup<A, X>
            : ISemigroup<Validation<A, X>>
        {
            public Validation<A, X> Op(Validation<A, X> t1, Validation<A, X> t2)
            {
                return t1.Cata(
                    a1 => t2,
                    xs1 => t2.Cata(
                        a2 => t1,
                        xs2 => Validation.Failure<A, X>(xs1.SemiOp(xs2))));
            }
        }

        private class NonEmptyLazyListSemigroup<A>
            : ISemigroup<NonEmptyLazyList<A>>
        {
            public NonEmptyLazyList<A> Op(NonEmptyLazyList<A> t1, NonEmptyLazyList<A> t2)
            {
                return new NonEmptyLazyList<A>(
                    t1.Head,
                    () => Jib.Maybe.Just(t1.Tail.Cata(nel => Op(nel, t2), () => t2)));
            }
        }
    }
}
