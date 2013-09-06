using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jib.Extensions;

namespace Jib.Tests
{
    [TestClass]
    public class FutureTest
    {
        class SpyTaskStrategy : AbstractStrategy
        {
            private readonly Action log;

            public SpyTaskStrategy(Action log)
            {
                this.log = log;
            }

            public override Func<A> Call<A>(Func<A> f)
            {
                log();
                return Strategies.Task.Call(f);
            }
        }

        [TestMethod]
        public void TestStrategies()
        {
            Assert.AreEqual(1, Future.Now(1).Run());
            Assert.AreEqual(1, Future.Lazy(new Lazy<int>(() => 1), Strategies.Task).Run());
            Assert.AreEqual(1, Future.Lazy(new Lazy<int>(() => 1), Strategies.Id).Run());
        }

        [TestMethod]
        public void TestMap()
        {
            Assert.AreEqual(4, Future.Now("asdf").Map(s => s.Length).Run());
        }

        [TestMethod]
        public void TestBind()
        {
            Assert.AreEqual(4, Future.Now("asdf").Bind(s => Future.Now(s.Length)).Run());
            Assert.AreEqual(3, Future.Now("asdf").Bind(s => Future.Now(s.Length).Bind(n => Future.Now(n - 1))).Run());
            Assert.AreEqual('s', Future.Now("asdf").Bind(s => Future.Now(1).Bind(n => Future.Now(s[n]))).Run());
        }

        [TestMethod]
        public void TestOrder()
        {
            var list = new List<int>();
            var expected = new List<int> {1, 2};
            var fa = Future.Func(() =>
                {
                    Thread.Sleep(10);
                    list.Add(1);
                    return 1;
                });
            var fb = Future.Func(() =>
                {
                    list.Add(2);
                    return 2;
                });
            CollectionAssert.AreEqual(expected, fa.Bind(a => fb.Bind(b => Future.Now(new List<int> {a, b}))).Run());
            CollectionAssert.AreEqual(expected, list);
        }

        [TestMethod]
        public void TestAp()
        {
            var tasks = 0;
            var strategy = new SpyTaskStrategy(() => ++tasks);
            var list = new List<int>();
            var expected = new List<int> {3, 2, 1};
            var ff = Future.Now<Func<int, Func<int, Func<int, int>>>>(a => b => c => a + b + c);
            var fa = Future.Func(() =>
                {
                    Thread.Sleep(10);
                    list.Add(1);
                    return 1;
                },
                strategy);
            var fb = Future.Func(() =>
                {
                    Thread.Sleep(5);
                    list.Add(2);
                    return 2;
                },
                strategy);
            var fc = Future.Func(() =>
                {
                    Thread.Sleep(0);
                    list.Add(3);
                    return 3;
                },
                strategy);
            Assert.AreEqual(6, Future.Strategy(ff, strategy).Ap(fa).Ap(fb).Ap(fc).Run());
            CollectionAssert.AreEqual(expected, list);
            Assert.AreEqual(4, tasks);
        }

        [TestMethod]
        public void TestLinq()
        {
            Assert.AreEqual(4, (from s in Future.Now("asdf") select s.Length).Run());
            // Fails with a type inference error...
            Future<int> fn = from s in Future.Now("asdf") from s1 in Future.Now(s.Length) select s1 - 1;
            Assert.AreEqual(3, fn.Run());
        }

        [TestMethod]
        public void TestMonadLaws()
        {
            Func<int, Future<int>> incr = i => Future.Now(i + 1);
            Func<int, Future<string>> str = i => Future.Now(i.ToString());
            // Left identity: Now(a).Bind(f) == f(a)
            Assert.AreEqual(Future.Now(1).Bind(incr).Run(), incr(1).Run());
            // Right identity: m.Bind(Now) == m
            Assert.AreEqual(Future.Now(1).Bind(Future.Now).Run(), 1);
            // Associativity: m.Bind(f).Bind(g) == m.Bind(x => f(x).Bind(g))
            Assert.AreEqual(
                    Future.Now(1).Bind(incr).Bind(str).Run(),
                    Future.Now(1).Bind(x => incr(x).Bind(str)).Run());
        }

        [TestMethod]
        public void TestCoMonadLaws()
        {
            // CoNow(CoJoin(m)) == m
            Assert.AreEqual(Future.Now(1).CoJoin().CoPure().Run(), 1);
            // CoJoin(m).Map(CoNow) == m
            Assert.AreEqual(Future.Now(1).CoJoin().Map(Future.CoPure).Run(), 1);
            // CoJoin(CoJoin(m)) == CoJoin(m).Map(CoJoin)
            Assert.AreEqual(
                    Future.Now(1).CoJoin().CoJoin().Run().Run().Run(),
                    Future.Now(1).CoJoin().Map(Future.CoJoin).Run().Run().Run());
        }
    }
}
