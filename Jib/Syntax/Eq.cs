using Jib.Instances;

namespace Jib.Syntax
{
    public static class MaybeEqSyntax
    {
        public static bool Eq<A>(this Maybe<A> maybe, Maybe<A> other, IEq<A> tEq)
        {
            return MaybeEq.Create(tEq).Eq(maybe, other);
        }
    }

    public static class EitherEqSyntax
    {
        public static bool Eq<X, A>(this Either<X, A> either, Either<X, A> other, IEq<X> xeq, IEq<A> aeq)
        {
            return EitherEq.Create(xeq, aeq).Eq(either, other);
        }
    }
}
