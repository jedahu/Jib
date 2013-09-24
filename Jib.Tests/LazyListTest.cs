using System.Collections.Generic;
using Jib.Syntax;
using NUnit.Framework;

namespace Jib.Tests
{
    [TestFixture]
    public class LazyListTests
        : SingleEnumerationTest
    {
        protected override IEnumerable<T> Create<T>(IEnumerable<T> input)
        {
            return input.LazyList().Enumerable();
        }
    }
}