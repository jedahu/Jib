using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public class PromiseTest
    {
        [Test]
        public void New_Promise_is_unfulfilled()
        {
            Assert.IsTrue(new Promise<int>().IsUnfulfilled);
        }

        [TestCase(1)]
        [TestCase(null)]
        [TestCase("abc")]
        public void Signalled_Promise_is_fulfilled(object value)
        {
            var p = new Promise<object>();
            p.Signal(value);
            Assert.IsTrue(p.IsFulfilled);
        }

        [TestCase(1)]
        [TestCase(null)]
        [TestCase("abc")]
        public void Signalled_Promise_throws_exception_on_Signal(object value)
        {
            var p = new Promise<object>();
            p.Signal(value);
            Assert.Throws<InvalidOperationException>(
                () => p.Signal(value),
                "Cannot signal fulfilled Promise.");
        }

        [Test]
        public void Wait_blocks_until_fulfilled()
        {
            var order = new List<int>();
            var p = new Promise<int>();
            var t1 = Task.Factory.StartNew(() => order.Add(p.Wait));
            var t2 = Task.Factory.StartNew(
                () =>
                    {
                        Thread.Sleep(100);
                        p.Signal(9);
                    });
            order.Add(1);
            Task.WaitAll(new[] {t1, t2});
            CollectionAssert.AreEqual(new[] {1, 9}, order.ToArray());
        }
    }
}
