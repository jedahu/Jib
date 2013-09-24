using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace Jib.Tests
{
    public class Generate
    {
        public readonly Random Random = new Random();

        public int NextNat
        {
            get { return Random.Next(1, 100); }
        }

        public IEnumerable<int> IntStream
        {
            get
            {
                while (true)
                {
                    yield return Random.Next();
                }
            }
        }

        public IEnumerable<int> Ints
        {
            get { return IntStream.Take(100); }
        }

        public IEnumerable<string> StringStream
        {
            get { return IntStream.Select(i => i.ToString(CultureInfo.InvariantCulture)); }
        }

        public IEnumerable<string> Strings
        {
            get { return StringStream.Take(100); }
        }

        public IEnumerable<string> NonEmptyStringStream
        {
            get { return StringStream.Where(s => !string.IsNullOrEmpty(s)); }
        }

        public IEnumerable<string> NonEmptyStrings
        {
            get { return NonEmptyStringStream.Take(100); }
        }

        public IEnumerable<object[]> IntStringStream
        {
            get { return IntStream.Zip(StringStream, (i, s) => new object[] {i, s}); }
        }

        public IEnumerable<object[]> IntStrings
        {
            get { return IntStringStream.Take(100); }
        }

        public IEnumerable<IEnumerable<int>> IntStreams
        {
            get
            {
                while (true)
                {
                    yield return IntStream;
                }
            }
        }

        public IEnumerable<int[]> Intss
        {
            get { return IntStreams.Select(xs => xs.Take(Random.Next(1, 100)).ToArray()).Take(100); }
        }
    }
}
