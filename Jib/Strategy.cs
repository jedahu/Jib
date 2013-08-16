using System;
using System.Threading.Tasks;

namespace Jib
{
    public interface Strategy
    {
        Func<A> Call<A>(Func<A> f);
        void Call(Action g);
    }

    public abstract class AbstractStrategy : Strategy
    {
        public void Call(Action g)
        {
            Call(Unit.Func(g));
        }

        public abstract Func<A> Call<A>(Func<A> f);
    }

    public static class Strategies
    {
        public static Strategy Task = new TaskStrategy();
        public static Strategy Id = new IdStrategy();
        public static Strategy Default = Task;
    }

    public class IdStrategy : AbstractStrategy
    {
        public override Func<A> Call<A>(Func<A> f)
        {
            var a = f();
            return () => a;
        }
    }

    public class TaskStrategy : AbstractStrategy
    {
        public override Func<A> Call<A>(Func<A> f)
        {
            var t = Task.Factory.StartNew(f);
            return () => t.Result;
        }
    }
}
