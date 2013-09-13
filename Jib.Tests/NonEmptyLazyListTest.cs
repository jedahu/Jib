using System.Collections.Generic;
using Jib.Extensions;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public class NonEmptyLazyListTests
        : SingleEnumerationTest
    {
        protected override IEnumerable<T> Create<T>(IEnumerable<T> input)
        {
            return input.NonEmptyLazyList().ValueOr(() => NonEmptyLazyList.Single(default(T))).Enumerable();
        }
    }
}