using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public abstract class SingleEnumerationTest
    {
        protected abstract IEnumerable<T> Create<T>(IEnumerable<T> input);

        protected static IEnumerable<int[]> RandomInts(int minimumLength = 0, int maximumLength = 1000)
        {
            var rand = new Random();
            return Enumerable.Repeat(
                Enumerable.Repeat<Func<int>>(
                    rand.Next,
                    rand.Next(minimumLength, maximumLength))
                    .Select(a => a())
                    .ToArray(),
                100);
        }

        protected static IEnumerable<string[]> RandomStringsAndNulls(int minimumLength = 0)
        {
            var rand = new Random();
            return Enumerable.Repeat(
                Enumerable.Repeat<Func<string>>(
                    () => rand.Next(0, 1) > 0 ? Path.GetRandomFileName() : null,
                    rand.Next(minimumLength, 1000))
                    .Select(a => a())
                    .ToArray(),
                100);
        }

        protected class RandomIntEnumerable : IEnumerable<int>
        {
            private readonly Random rand = new Random();

            public IEnumerator<int> GetEnumerator()
            {
                for (var i = 0; i < 100; ++i)
                {
                    yield return rand.Next();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Test]
        public void Enumerates_value_types()
        {
            foreach (var expected in RandomInts())
            {
                CollectionAssert.AreEqual(expected, Create(expected).ToArray());
            }
        }

        [Test]
        public void Enumerates_reference_types()
        {
            foreach (var expected in RandomStringsAndNulls())
            {
                CollectionAssert.AreEqual(expected, Create(expected).ToArray());
            }
        }

        [Test]
        public void Enumerates_twice()
        {
            foreach (var expected in RandomInts())
            {
                var memo = Create(expected);
                CollectionAssert.AreEqual(expected, memo.ToArray());
                CollectionAssert.AreEqual(expected, memo.ToArray());
            }
        }

        [Test]
        public void Enumerates_partially_then_fully()
        {
            foreach (var expected in RandomInts(10))
            {
                var memo = Create(expected);
                CollectionAssert.AreEqual(expected.Take(9), memo.Take(9));
                var actual = memo.ToArray();
                Assert.AreEqual(expected.Length, actual.Length);
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Runs_through_source_enumerator_only_once()
        {
            foreach (var enumerable in Enumerable.Repeat<Func<RandomIntEnumerable>>(() => new RandomIntEnumerable(), 100).Select(a => a()))
            {
                var memo = Create(enumerable);
                var partialRun = memo.Take(9).ToArray();
                var fullRun = memo.ToArray();
                CollectionAssert.AreEqual(partialRun, fullRun.Take(9).ToArray());
                CollectionAssert.AreEqual(fullRun, memo.ToArray());
            }
        }

        [Test]
        public void Fails_when_source_enumerable_is_modified_before_full_enumeration_is_complete()
        {
            var rand = new Random();
            foreach (var enumerable in RandomInts(5, 10))
            {
                var list = enumerable.ToList();
                foreach (var memo in Enumerable.Range(0, 10).Select(i => Create(list)))
                {
                    var memo1 = memo;
                    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                    memo1.Take(rand.Next(3, 9));
                    list.Add(6);
                    Assert.Throws<InvalidOperationException>(() => memo1.ToArray());
                    // ReSharper enable ReturnValueOfPureMethodIsNotUsed
                }
            }
        }

        [Test]
        public void Does_not_fail_when_source_enumerable_is_modified_after_full_enumeration_is_complete()
        {
            foreach (var enumerable in RandomInts(5, 10))
            {
                var list = enumerable.ToList();
                foreach (var memo in Enumerable.Range(0, 10).Select(i => Create(list)))
                {
                    var memo1 = memo;
                    memo1.ToArray();
                    list.Add(6);
                    Assert.DoesNotThrow(() => memo1.ToArray());
                }
            }
        }

        [Test]
        public void Enumerator_is_not_resettable_before_full_enumeration_is_complete()
        {
            Assert.Throws<NotSupportedException>(() => Create(new[] {1, 2}).GetEnumerator().Reset());
        }

        [Test]
        public void Enumerator_is_not_resettable_after_full_enumeration_is_complete()
        {
            var memo = Create(new[] {1, 2});
            memo.ToArray();
            Assert.Throws<NotSupportedException>(() => memo.GetEnumerator().Reset());
        }
    }
}