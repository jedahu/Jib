using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jib.Instances
{
    public class FuncFunctor
    {
        private FuncFunctor() { }

        public Func<A, C> Map<A, B, C>(Func<A, B> g, Func<B, C> f)
        {
            return x => f(g(x));
        }

        public static FuncFunctor Instance = new FuncFunctor();

        public static IFunctor<B> Dynamic<A, B>(Func<A, B> g)
        {
            return new DynamicFunctor<A, B>(g);
        }

        private class DynamicFunctor<A, B>
            : IFunctor<B>
        {
            private readonly Func<A, B> g;

            public DynamicFunctor(Func<A, B> g)
            {
                this.g = g;
            }

            public IFunctor<C> Map<C>(Func<B, C> f)
            {
                return new DynamicFunctor<A, C>(Instance.Map(g, f));
            }

            public object Value { get { return g; } }
        }
    }

    public class MaybeFunctor
    {
        private MaybeFunctor() { }

        public Maybe<B> Map<A, B>(Maybe<A> maybe, Func<A, B> f)
        {
            return maybe.Cata(Maybe.Nothing<B>, a => Maybe.Just(f(a)));
        }

        public static MaybeFunctor Instance = new MaybeFunctor();

        public static IFunctor<A> Dynamic<A>(Maybe<A> maybe)
        {
            return new DynamicFunctor<A>(maybe);
        }

        private class DynamicFunctor<A>
            : IFunctor<A>
        {
            private readonly Maybe<A> maybe;

            public DynamicFunctor(Maybe<A> maybe)
            {
                this.maybe = maybe;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<B>(Instance.Map(maybe, f));
            }

            public object Value
            {
                get { return maybe; }
            }
        }
    }

    public class EitherFunctor
    {
        private EitherFunctor() { }

        public Either<X, B> Map<X, A, B>(Either<X, A> either, Func<A, B> f)
        {
            return either.Cata(Either.Left<X, B>, a => Either.Right<X, B>(f(a)));
        }

        public Either<Y, A> MapLeft<X, Y, A>(Either<X, A> either, Func<X, Y> f)
        {
            return either.Swap(e => Map(e, f));
        }

        public static EitherFunctor Instance = new EitherFunctor();

        public static IFunctor<A> Dynamic<X, A>(Either<X, A> either)
        {
            return new DynamicFunctor<X, A>(either);
        }

        private class DynamicFunctor<X, A>
            : IFunctor<A>
        {
            private readonly Either<X, A> either;

            public DynamicFunctor(Either<X, A> either)
            {
                this.either = either;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<X, B>(Instance.Map(either, f));
            }

            public object Value
            {
                get { return either; }
            }
        }
    }

    public class ValidationFunctor
    {
        private ValidationFunctor() { }

        public Validation<X, B> Map<X, A, B>(Validation<X, A> validation, Func<A, B> f)
        {
            return validation.Cata(Validation.Failure<X, B>, a => Validation.Success<X, B>(f(a)));
        }

        public Validation<Y, A> MapFailure<X, Y, A>(Validation<X, A> validation, Func<X, Y> f)
        {
            return validation.Cata(
                xs => Validation.Failure<Y, A>(
                    NonEmptyLazyListFunctor.Instance.Map(xs, f)),
                    Validation.Success<Y, A>);
        }

        public static ValidationFunctor Instance = new ValidationFunctor();

        public static IFunctor<A> Dynamic<X, A>(Validation<X, A> validation)
        {
            return new DynamicFunctor<X, A>(validation);
        }

        private class DynamicFunctor<X, A>
            : IFunctor<A>
        {
            private readonly Validation<X, A> validation;

            public DynamicFunctor(Validation<X, A> validation)
            {
                this.validation = validation;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<X, B>(Instance.Map(validation, f));
            }

            public object Value
            {
                get { return validation; }
            }
        }
    }

    public class PromiseFunctor
    {
        private PromiseFunctor() { }

        public Promise<B> Map<A, B>(Promise<A> promise, Func<A, B> f)
        {
            var p = new Promise<B>();
            p.Strategy.Call(() => p.Signal(f(promise.Wait)));
            return p;
        }

        public static PromiseFunctor Instance = new PromiseFunctor();

        public static IFunctor<A> Dynamic<A>(Promise<A> promise)
        {
            return new DynamicFunctor<A>(promise);
        }

        private class DynamicFunctor<A>
            : IFunctor<A>
        {
            private readonly Promise<A> promise;

            public DynamicFunctor(Promise<A> promise)
            {
                this.promise = promise;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<B>(Instance.Map(promise, f));
            }

            public object Value
            {
                get { return promise; }
            }
        }
    }

    public class FutureFunctor
    {
        private FutureFunctor() { }

        public Future<B> Map<A, B>(Future<A> future, Func<A, B> f)
        {
            return new Future<B>(
                cb => future.Callback(a => cb(f(a))),
                future.Strategy);
        }

        public static FutureFunctor Instance = new FutureFunctor();

        public static IFunctor<A> Dynamic<A>(Future<A> future)
        {
            return new DynamicFunctor<A>(future);
        }

        private class DynamicFunctor<A>
            : IFunctor<A>
        {
            private readonly Future<A> future;

            public DynamicFunctor(Future<A> future)
            {
                this.future = future;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<B>(Instance.Map(future, f));
            }

            public object Value
            {
                get { return future; }
            }
        }
    }

    public class TaskFunctor
    {
        private TaskFunctor() { }

        public Task<B> Map<A, B>(Task<A> task, Func<A, B> f)
        {
            return task.ContinueWith(t => f(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static TaskFunctor Instance = new TaskFunctor();

        public static IFunctor<A> Dynamic<A>(Task<A> task)
        {
            return new DynamicFunctor<A>(task);
        }

        private class DynamicFunctor<A>
            : IFunctor<A>
        {
            private readonly Task<A> task;

            public DynamicFunctor(Task<A> task)
            {
                this.task = task;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<B>(Instance.Map(task, f));
            }

            public object Value
            {
                get { return task; }
            }
        }
    }

    public class NonEmptyLazyListFunctor
    {
        private NonEmptyLazyListFunctor() { }

        public NonEmptyLazyList<B> Map<A, B>(NonEmptyLazyList<A> list, Func<A, B> f)
        {
            var ht = list.HeadTail();
            return f(ht.Fst).Cons(ht.Snd.Select(f));
        }

        public static NonEmptyLazyListFunctor Instance = new NonEmptyLazyListFunctor();

        public static IFunctor<A> Dynamic<A>(NonEmptyLazyList<A> list)
        {
            return new DynamicFunctor<A>(list);
        }

        private class DynamicFunctor<A>
            : IFunctor<A>
        {
            private readonly NonEmptyLazyList<A> list;

            public DynamicFunctor(NonEmptyLazyList<A> list)
            {
                this.list = list;
            }

            public IFunctor<B> Map<B>(Func<A, B> f)
            {
                return new DynamicFunctor<B>(Instance.Map(list, f));
            }

            public object Value
            {
                get { return list; }
            }
        }
    }
}
