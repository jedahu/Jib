using System;

namespace Jib
{
    public static class Either
    {
        public static Either<X, A> Left<X, A>(X leftValue)
        {
            return new Either<X, A>(leftValue, default(A), false);
        }

        public static Either<X, A> Right<X, A>(A rightValue)
        {
            return new Either<X, A>(default(X), rightValue, true);
        }

        public static Either<X, A> LeftIf<X, A>(bool isLeft, Func<X> left, Func<A> right)
        {
            return isLeft ? Left<X, A>(left()) : Right<X, A>(right());
        }

        public static Either<X, A> RightIf<X, A>(bool isRight, Func<X> left, Func<A> right)
        {
            return isRight ? Right<X, A>(right()) : Left<X, A>(left());
        }

        public static Either<X, A> LeftIf<X, A>(bool isLeft, X left, A right)
        {
            return isLeft ? Left<X, A>(left) : Right<X, A>(right);
        }

        public static Either<X, A> RightIf<X, A>(bool isRight, X left, A right)
        {
            return isRight ? Right<X, A>(right) : Left<X, A>(left);
        }

        public static bool IsLeft<X, A>(this Either<X, A> either)
        {
            return either.Cata(x => true, a => false);
        }

        public static bool IsRight<X, A>(this Either<X, A> either)
        {
            return either.Cata(x => false, a => true);
        }

        public static X LeftOr<X, A>(this Either<X, A> either, Func<X> or)
        {
            return either.Cata(x => x, a => or());
        }

        public static A RightOr<X, A>(this Either<X, A> either, Func<A> or)
        {
            return either.Cata(x => or(), a => a);
        }

        public static Either<A, X> Swap<X, A>(this Either<X, A> either)
        {
            return either.Cata(Right<A, X>, Left<A, X>);
        }

        public static Either<Y, A> Swap<A, X, Y>(this Either<X, A> either, Func<Either<A, X>, Either<A, Y>> f)
        {
            return f(either.Swap()).Swap();
        }
    }
}