using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jib.Tests
{
    [TestClass]
    public class StrategyTest
    {
        List<int> EvalOrder(Strategy s)
        {
            var order = new List<int>();
            var expected = new List<int> {1, 2, 3};
            Func<Func<int>> task = () =>
            {
                order.Add(1);
                return () =>
                {
                    Thread.Sleep(5);
                    order.Add(2);
                    return 3;
                };
            };
            var f = s.Call(task());
            order.Add(f());
            return order;
        }

        [TestMethod]
        public void TestEvaluationOrder()
        {
            var expected = new List<int> {1, 2, 3};
            CollectionAssert.AreEqual(expected, EvalOrder(Strategies.Id));
            CollectionAssert.AreEqual(expected, EvalOrder(Strategies.Task));
        }
    }
}
