using System;
using System.Collections.Generic;

namespace Jib.Syntax
{
    public static class MaybeExtend
    {
        public static Maybe<B> Extend<A, B>(this Maybe<A> maybe, Func<Maybe<A>, B> f)
        {
            return f(maybe).PureMaybe();
        }

        public static Maybe<Maybe<A>> Duplicate<A>(this Maybe<A> maybe)
        {
            return maybe.PureMaybe();
        }
    }

    public static class EitherExtend
    {
        public static Either<B, X> Extend<A, B, X>(this Either<A, X> either, Func<Either<A, X>, B> f)
        {
            return f(either).PureEither<B, X>();
        }

        public static Either<Either<A, X>, X> Duplicate<A, X>(this Either<A, X> either)
        {
            return either.PureEither<Either<A, X>, X>();
        }
    }

    public static class ValidationExtend
    {
        public static Validation<X, B> Extend<X, A, B>(this Validation<X, A> validation, Func<Validation<X, A>, B> f)
        {
            return f(validation).PureValidation<X, B>();
        }

        public static Validation<X, Validation<X, A>> Duplicate<X, A>(this Validation<X, A> validation)
        {
            return validation.PureValidation<X, Validation<X, A>>();
        }
    }

    public static class PromiseExtend
    {
        public static Promise<B> Extend<A, B>(this Promise<A> promise, Func<Promise<A>, B> f)
        {
            return f(promise).PurePromise();
        }

        public static Promise<Promise<A>> Duplicate<A>(this Promise<A> promise)
        {
            return promise.PurePromise();
        }
    }

    public static class NonEmptyLazyListExtend
    {
        public static NonEmptyLazyList<B> Extend<A, B>(this NonEmptyLazyList<A> list, Func<NonEmptyLazyList<A>, B> f)
        {
            return f(list).PureNonEmptyLazyList();
        }

        //public static NonEmptyLazyList<NonEmptyLazyList<A>> Duplicate<A>(this NonEmptyLazyList<A> list)
        //{
        //    return list.Tails();
        //}
    }
}
