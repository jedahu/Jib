using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jib.Syntax
{
    public static class MaybeExtract
    {
        public static A Extract<A>(this Maybe<A> maybe, IMonoid<A> m)
        {
            return maybe.ValueOr(() => m.Zero);
        }
    }

    public static class EitherExtract
    {
        public static A Extract<X, A>(this Either<X, A> either, IMonoid<A> m)
        {
            return either.RightOr(() => m.Zero);
        }
    }

    public static class ValidationExtract
    {
        public static A Extract<X, A>(this Validation<X, A> validation, IMonoid<A> m)
        {
            return validation.SuccessOr(() => m.Zero);
        }
    }

    public static class PromiseExtract
    {
        public static A Extract<A>(this Promise<A> promise)
        {
            return promise.Wait;
        }
    }

    public static class FutureExtract
    {
        public static A Extract<A>(this Future<A> future)
        {
            return future.Run();
        }
    }

    public static class TaskExtract
    {
        public static A Extract<A>(this Task<A> task)
        {
            return task.Result;
        }
    }

    public static class EnumerableExtract
    {
        public static A Extract<A>(this IEnumerable<A> enumerable, IMonoid<A> m)
        {
            return enumerable.MaybeFirst().ValueOr(() => m.Zero);
        }
    }

    public static class NonEmptyLazyListExtract
    {
        public static A Extract<A>(this NonEmptyLazyList<A> list)
        {
            return list.Head;
        }
    }
}
