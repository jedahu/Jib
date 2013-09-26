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

        protected virtual int Finite
        {
            get { return 100; }
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
            get { return IntStream.Take(Finite); }
        }

        public IEnumerable<char> CharStream
        {
            get { return IntStream.Select(Convert.ToChar); }
        }

        public IEnumerable<char> Chars
        {
            get { return CharStream.Take(Finite); }
        }

        public IEnumerable<string> StringStream
        {
            get { return IntStream.Select(i => i.ToString(CultureInfo.InvariantCulture)); }
        }

        public IEnumerable<string> Strings
        {
            get { return StringStream.Take(Finite); }
        }

        public IEnumerable<string> NonEmptyStringStream
        {
            get { return StringStream.Where(s => !string.IsNullOrEmpty(s)); }
        }

        public IEnumerable<string> NonEmptyStrings
        {
            get { return NonEmptyStringStream.Take(Finite); }
        }

        public IEnumerable<object[]> IntStringStream
        {
            get { return IntStream.Zip(StringStream, (i, s) => new object[] {i, s}); }
        }

        public IEnumerable<object[]> IntStrings
        {
            get { return IntStringStream.Take(Finite); }
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
            get { return IntStreams.Select(xs => xs.Take(Random.Next(1, 100)).ToArray()).Take(Finite); }
        }

        public IEnumerable<IEnumerable<char>> CharStreams
        {
            get
            {
                while (true)
                {
                    yield return CharStream;
                }
            }
        }

        public IEnumerable<char[]> Charss
        {
            get { return CharStreams.Select(cs => cs.Take(Random.Next(1, 100)).ToArray()).Take(Finite); }
        }
    }
}
