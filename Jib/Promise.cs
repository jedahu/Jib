using System;
using System.Threading;

namespace Jib
{
    public class Promise<T>
        : Unobject
    {
        private T value;
        private readonly IStrategy strategy;
        private readonly Action<T> fulfillAction;
        private readonly CountdownEvent writeLatch = new CountdownEvent(1);
        private readonly CountdownEvent readLatch = new CountdownEvent(1);

        public Promise(Action<T> onFulfill, IStrategy strategy)
        {
            this.strategy = strategy;
            fulfillAction = onFulfill;
            writeLatch = new CountdownEvent(1);
            readLatch = new CountdownEvent(1);
        }

        public Promise()
            : this(t => { }, Strategies.Default)
        {
        }

        public IStrategy Strategy
        {
            get { return strategy; }
        }

        public T Wait
        {
            get
            {
                readLatch.Wait();
                return value;
            }
        }

        public void Signal(T v)
        {
            try
            {
                writeLatch.Signal();
                value = v;
                readLatch.Signal();
                fulfillAction(value);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Cannot signal fulfilled Promise.",
                    ex);
            }
        }

        public bool IsFulfilled
        {
            get { return writeLatch.CurrentCount == 0; }
        }

        public bool IsUnfulfilled
        {
            get { return !IsFulfilled; }
        }
    }

    public static class Promise
    {
        public static Promise<T> Now<T>(T value)
        {
            var p = new Promise<T>();
            p.Signal(value);
            return p;
        }
    }
}
