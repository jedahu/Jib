namespace Jib
{
    public interface IEq<A>
    {
        bool Eq(A t1, A t2);
    }

    public static class Eq
    {
        public static IEq<Maybe<A>> Maybe<A>(IEq<A> teq)
        {
            return new MaybeEq<A>(teq);
        }

        public static IEq<Either<A, X>> Either<A, X>(IEq<A> teq, IEq<X> xeq)
        {
            return new EitherEq<A, X>(teq, xeq);
        }

        public static IEq<A> Struct<A>() where A : struct
        {
            return new StructEq<A>();
        }

        public static IEq<A> Class<A>() where A : class
        {
            return new ClassEq<A>();
        }

        private class ClassEq<A>
            : IEq<A>
            where A : class
        {
            public bool Eq(A t1, A t2)
            {
                return t1 == null ? t2 == null : t1.Equals(t2);
            }
        }

        private class StructEq<A>
            : IEq<A>
            where A : struct
        {
            public bool Eq(A t1, A t2)
            {
                return t1.Equals(t2);
            }
        }

        private class MaybeEq<A>
            : IEq<Maybe<A>>
        {
            private readonly IEq<A> teq;

            public MaybeEq(IEq<A> teq)
            {
                this.teq = teq;
            }

            public bool Eq(Maybe<A> t1, Maybe<A> t2)
            {
                return t1.Cata(
                    a1 => t2.Cata(
                              a2 => teq.Eq(a1, a2),
                              () => false),
                    () => t2.Cata(
                        a2 => false,
                        () => true));
            }
        }

        private class EitherEq<A, X>
            : IEq<Either<A, X>>
        {
            private readonly IEq<A> teq;
            private readonly IEq<X> xeq;

            public EitherEq(IEq<A> teq, IEq<X> xeq)
            {
                this.teq = teq;
                this.xeq = xeq;
            }

            public bool Eq(Either<A, X> t1, Either<A, X> t2)
            {
                return t1.Cata(
                    a1 => t2.Cata(
                        a2 => teq.Eq(a1, a2),
                        x2 => false),
                    x1 => t2.Cata(
                        a2 => false,
                        x2 => xeq.Eq(x1, x2)));
            }
        }
    }
}
