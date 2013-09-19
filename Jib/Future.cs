using System;
using System.Threading;

namespace Jib
{
    public class Future<A>
        : Unobject
    {
        private readonly Action<Action<A>> callback;

        private readonly IStrategy strategy;

        public Future(Action<Action<A>> callback, IStrategy strategy = null)
        {
            this.callback = callback;
            this.strategy = strategy ?? Strategies.Default;
        }

        internal Action<Action<A>> Callback
        {
            get { return callback; }
        }

        public IStrategy Strategy
        {
            get { return strategy; }
        }

        public void RunAsync(Action<A> action)
        {
            Strategy.Call(() => Callback(action));
        }

        public A Run()
        {
            var latch = new CountdownEvent(1);
            var call = Strategy.Call(() =>
                {
                    A result = default(A);
                    Callback(a =>
                        {
                            result = a;
                            latch.Signal();
                        });
                    latch.Wait();
                    return result;
                });
            return call();
        }
    }
}
