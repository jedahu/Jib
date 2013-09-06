using System;
using System.Collections.Generic;
using System.Linq;
using Jib.Extensions;

namespace Jib
{
    public static class Either
    {
        public static Either<A, X> Left<A, X>(A leftValue)
        {
            return new Either<A, X>(leftValue, default(X), true);
        }

        public static Either<A, X> Right<A, X>(X rightValue)
        {
            return new Either<A, X>(default(A), rightValue, false);
        }

        public static Either<A, X> LeftIf<A, X>(bool isLeft, Func<A> left, Func<X> right)
        {
            return isLeft ? Left<A, X>(left()) : Right<A, X>(right());
        }

        public static Either<A, X> RightIf<A, X>(bool isRight, Func<A> left, Func<X> right)
        {
            return isRight ? Right<A, X>(right()) : Left<A, X>(left());
        }

        public static Either<A, X> LeftIf<A, X>(bool isLeft, A left, X right)
        {
            return isLeft ? Left<A, X>(left) : Right<A, X>(right);
        }

        public static Either<A, X> RightIf<A, X>(bool isRight, A left, X right)
        {
            return isRight ? Right<A, X>(right) : Left<A, X>(left);
        }

        public static bool IsLeft<A, X>(this Either<A, X> either)
        {
            return either.Cata(a => true, x => false);
        }

        public static bool IsRight<A, X>(this Either<A, X> either)
        {
            return either.Cata(a => false, x => true);
        }

        public static A LeftOr<A, X>(this Either<A, X> either, Func<A> or)
        {
            return either.Cata(a => a, x => or());
        }

        public static X RightOr<A, X>(this Either<A, X> either, Func<X> or)
        {
            return either.Cata(a => or(), x => x);
        }

        public static Either<X, A> Swap<A, X>(this Either<A, X> either)
        {
            return either.Cata(Right<X, A>, Left<X, A>);
        }

        public static Either<A, Y> Swap<A, X, Y>(this Either<A, X> either, Func<Either<X, A>, Either<Y, A>> f)
        {
            return f(either.Swap()).Swap();
        }

        public static IEnumerable<A> Lefts<A, X>(this IEnumerable<Either<A, X>> eithers)
        {
            return eithers.SelectMany(e => e.LeftEnumerable());
        }

        public static IEnumerable<X> Rights<A, X>(this IEnumerable<Either<A, X>> eithers)
        {
            return eithers.SelectMany(e => e.RightEnumerable());
        }
    }
}