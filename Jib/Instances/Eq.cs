namespace Jib.Instances
{
    public class ValueEq<A>
        : IEq<A>
        where A : struct
    {
        public bool Eq(A a1, A a2)
        {
            return a1.Equals(a2);
        }
    }

    public class RefEq<A>
        : IEq<A>
        where A : class
    {
        public bool Eq(A a1, A a2)
        {
            return a1 == null ? a2 == null : a1.Equals(a2);
        }
    }

    public static class PrimEq
    {
        public static readonly IEq<int> Int = new ValueEq<int>();
        public static readonly IEq<double> Double = new ValueEq<double>();
        public static readonly IEq<char> Char = new ValueEq<char>();
        public static readonly IEq<string> String = new RefEq<string>();
    }

    public class MaybeEq<A>
        : IEq<Maybe<A>>
    {
        private readonly IEq<A> teq;

        public MaybeEq(IEq<A> teq)
        {
            this.teq = teq;
        }

        public bool Eq(Maybe<A> t1, Maybe<A> t2)
        {
            return t1.Cata(() => t2.Cata(() => true, a2 => false), a1 => t2.Cata(() => false, a2 => teq.Eq(a1, a2)));
        }
    }

    public static class MaybeEq
    {
        public static IEq<Maybe<A>> Create<A>(IEq<A> aeq)
        {
            return new MaybeEq<A>(aeq);
        }
    }

    public class EitherEq<X, A>
        : IEq<Either<X, A>>
    {
        private readonly IEq<X> xeq;
        private readonly IEq<A> aeq;

        public EitherEq(IEq<X> xeq, IEq<A> aeq)
        {
            this.xeq = xeq;
            this.aeq = aeq;
        }

        public bool Eq(Either<X, A> t1, Either<X, A> t2)
        {
            return t1.Cata(
                x1 => t2.Cata(
                    x2 => xeq.Eq(x1, x2),
                    a2 => false),
                a1 => t2.Cata(
                    x2 => false,
                    a2 => aeq.Eq(a1, a2)));
        }
    }

    public static class EitherEq
    {
        public static IEq<Either<X, A>> Create<X, A>(IEq<X> xeq, IEq<A> aeq)
        {
            return new EitherEq<X, A>(xeq, aeq);
        }
    }

    public class ValidationEq<X, A>
        : IEq<Validation<X, A>>
    {
        private readonly IEq<A> aeq;
        private readonly IEq<NonEmptyLazyList<X>> neq;

        public ValidationEq(IEq<X> xeq, IEq<A> aeq)
        {
            this.aeq = aeq;
            neq = NonEmptyLazyListEq.Create(xeq);
        }

        public bool Eq(Validation<X, A> v1, Validation<X, A> v2)
        {
            return v1.Cata(
                xs1 => v2.Cata(
                    xs2 => neq.Eq(xs1, xs2),
                    a1 => false),
                a1 => v2.Cata(
                    xs2 => false,
                    a2 => aeq.Eq(a1, a2)));
        }
    }

    public static class ValidationEq
    {
        public static IEq<Validation<X, A>> Create<X, A>(IEq<X> xeq, IEq<A> aeq)
        {
            return new ValidationEq<X, A>(xeq, aeq);
        }
    }

    public class NonEmptyLazyListEq<A>
        : IEq<NonEmptyLazyList<A>>
    {
        private readonly IEq<A> aeq;

        public NonEmptyLazyListEq(IEq<A> aeq)
        {
            this.aeq = aeq;
        }

        public bool Eq(NonEmptyLazyList<A> t1, NonEmptyLazyList<A> t2)
        {
            return
                aeq.Eq(t1.Head, t2.Head) &&
                ((t1.Tail.IsNothing() && t2.Tail.IsNothing())
                 ||
                 MaybeFunctor.Instance.Map(
                     MaybeZipable.Instance.Zip(t1.Tail, t2.Tail),
                     p => Eq(p.Fst, p.Snd))
                     .ValueOr(() => false));
        }
    }

    public static class NonEmptyLazyListEq
    {
        public static IEq<NonEmptyLazyList<A>> Create<A>(IEq<A> aeq)
        {
            return new NonEmptyLazyListEq<A>(aeq);
        }
    }
}
