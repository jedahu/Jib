using System;
using System.Linq;
using System.Threading.Tasks;
using Jib.Instances;

namespace Jib.Syntax
{
    public static class FuncFunctorSyntax
    {
        public static Func<X, B> Map<X, A, B>(this Func<X, A> g, Func<A, B> f)
        {
            return FuncFunctor.Instance.Map(g, f);
        }
    }

    public static class MaybeFunctorSyntax
    {
        public static Maybe<B> Map<A, B>(this Maybe<A> maybe, Func<A, B> f)
        {
            return MaybeFunctor.Instance.Map(maybe, f);
        }
    }

    public static class EitherFunctorSyntax
    {
        public static Either<X, B> Map<X, A, B>(this Either<X, A> either, Func<A, B> f)
        {
            return EitherFunctor.Instance.Map(either, f);
        }

        public static Either<Y, A> MapRight<X, Y, A>(this Either<X, A> either, Func<X, Y> f)
        {
            return EitherFunctor.Instance.MapLeft(either, f);
        }
    }

    public static class ValidationFunctorSyntax
    {
        public static Validation<X, B> Map<X, A, B>(this Validation<X, A> validation, Func<A, B> f)
        {
            return ValidationFunctor.Instance.Map(validation, f);
        }

        public static Validation<Y, A> MapFailure<X, Y, A>(this Validation<X, A> validation, Func<X, Y> f)
        {
            return ValidationFunctor.Instance.MapFailure(validation, f);
        }
    }

    public static class PromiseFunctorSyntax
    {
        public static Promise<B> Map<A, B>(this Promise<A> promise, Func<A, B> f)
        {
            return PromiseFunctor.Instance.Map(promise, f);
        }
    }

    public static class FutureFunctorSyntax
    {
        public static Future<B> Map<A, B>(this Future<A> future, Func<A, B> f)
        {
            return FutureFunctor.Instance.Map(future, f);
        }
    }

    public static class TaskFunctorSyntax
    {
        public static Task<B> Map<A, B>(this Task<A> task, Func<A, B> f)
        {
            return TaskFunctor.Instance.Map(task, f);
        }
    }

    public static class NonEmptyLazyListFunctorSyntax
    {
        public static NonEmptyLazyList<B> Map<A, B>(this NonEmptyLazyList<A> list, Func<A, B> f)
        {
            return NonEmptyLazyListFunctor.Instance.Map(list, f);
        }
    }
}