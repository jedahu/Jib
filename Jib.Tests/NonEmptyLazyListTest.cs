using System.Collections.Generic;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public class NonEmptyLazyListTests
        : SingleEnumerationTest
    {
        protected override IEnumerable<T> Create<T>(IEnumerable<T> input)
        {
            return input.MaybeNonEmptyLazyList().ValueOr(NonEmptyLazyList.Single(default(T)));
        }
    }
}