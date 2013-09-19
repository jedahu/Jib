using System;
using System.Threading.Tasks;

namespace Jib
{
    public static class Strategies
    {
        public static IStrategy Task = new TaskStrategy();
        public static IStrategy Id = new IdStrategy();
        public static IStrategy Default = Task;
    }

    public abstract class AbstractStrategy
        : Unobject
        , IStrategy
    {
        public void Call(Action g)
        {
            Call(Unit.Func(g));
        }

        public abstract Func<A> Call<A>(Func<A> f);
    }

    public class IdStrategy
        : AbstractStrategy
    {
        public override Func<A> Call<A>(Func<A> f)
        {
            var a = f();
            return () => a;
        }
    }

    public class TaskStrategy
        : AbstractStrategy
    {
        public override Func<A> Call<A>(Func<A> f)
        {
            var t = Task.Factory.StartNew(f);
            return () => t.Result;
        }
    }
}
