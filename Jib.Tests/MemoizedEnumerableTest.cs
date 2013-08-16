using System.Collections.Generic;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public class MemoizedEnumerableTests
        : SingleEnumerationTest
    {
        protected override IEnumerable<T> Create<T>(IEnumerable<T> input)
        {
            return input.Memoize();
        }
    }
}