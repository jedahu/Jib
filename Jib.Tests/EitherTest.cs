using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Jib.Extensions;
using Jib.Test;
using NUnit.Framework;

namespace Jib.Tests
{
    public class EitherTest
    {
        #region Construction

        [Test]
        public void Left_constructs_a_left_Either()
        {
            Assert.IsTrue(Either.Left<int, string>(3).IsLeft());
        }

        [Test]
        public void Right_constructs_a_right_Either()
        {
            Assert.IsTrue(Either.Right<int, string>("abc").IsRight());
        }

        [Test]
        public void Constructors_allow_null()
        {
            Assert.DoesNotThrow(() => Either.Left<string, string>(null));
            Assert.DoesNotThrow(() => Either.Right<int, string>(null));
        }

        #endregion

        #region Equality

        [TestCase(1, 1)]
        [TestCase(-3, 2)]
        public void Left_equals_Left(int i1, int i2)
        {
            var result = Eq.Either(Eq.Int, Eq.String).Eq(
                Either.Left<int, string>(i1),
                Either.Left<int, string>(i2));
            if (Eq.Int.Eq(i1, i2)) Assert.IsTrue(result);
            else Assert.IsFalse(result);
        }

        [TestCase("a", "a")]
        [TestCase("", "m")]
        [TestCase(null, null)]
        [TestCase("x", null)]
        public void Right_equals_Right(string s1, string s2)
        {
            var result = Eq.Either(Eq.Int, Eq.String).Eq(
                Either.Right<int, string>(s1),
                Either.Right<int, string>(s2));
            if (Eq.String.Eq(s1, s2)) Assert.IsTrue(result);
            else Assert.IsFalse(result);
        }

        [TestCase("a", "a")]
        [TestCase("x", "y")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Left_symmetrically_does_not_equal_Right(string s1, string s2)
        {
            Assert.IsFalse(
                Eq.Either(Eq.String, Eq.String).Eq(
                Either.Left<string, string>(s1),
                Either.Right<string, string>(s2)));
            Assert.IsFalse(
                Eq.Either(Eq.String, Eq.String).Eq(
                Either.Right<string, string>(s1),
                Either.Left<string, string>(s2)));
        }

        #endregion

        #region Fold

        [Test]
        public void Fold_on_Left_calls_left_func()
        {
            Assert.AreEqual(3, Either.Left<string, int>("abc").Cata(a => a.Length, x => x));
        }

        [Test]
        public void Fold_on_Right_calls_right_func()
        {
            Assert.AreEqual(2, Either.Right<string, int>(2).Cata(a => a.Length, x => x));
        }

        [Test]
        public void FoldVoid_on_Left_calls_left_action()
        {
            var value = -1;
            Either.Left<string, int>("abc").CataVoid(a => value = a.Length, x => value = x);
            Assert.AreEqual(3, value);
        }

        [Test]
        public void FoldVoid_on_right_calls_right_action()
        {
            var value = -1;
            Either.Right<string, int>(2).CataVoid(a => value = a.Length, x => value = x);
            Assert.AreEqual(2, value);
        }

        #endregion

        #region Linq

        [Test]
        public void Linq_left_identity()
        {
            var eq = Eq.Either(Eq.Int, Eq.String);
            Func<IList<int>, Either<int, string>> first =
                list => list.Count > 0 ?
                    Either.Left<int, string>(list[0]) :
                    Either.Right<int, string>("Empty list.");
            Assert.True(
                eq.Eq(
                    Either.Left<IList<int>, string>(new List<int> {3, 2}).Bind(first),
                    first(new List<int> {3, 2})));
            Assert.True(
                eq.Eq(
                    Either.Left<IList<int>, string>(new List<int>()).Bind(first),
                    first(new List<int>())));
        }

        [Test]
        public void Linq_right_identity()
        {
            var eq = Eq.Either(Eq.Int, Eq.String);
            Assert.True(
                eq.Eq(
                    Either.Left<int, string>(3).Bind(Either.Left<int, string>),
                    Either.Left<int, string>(3)));
            Assert.True(
                eq.Eq(
                    Either.Right<int, string>("error").Bind(Either.Left<int, string>),
                    Either.Right<int, string>("error")));
        }

        [Test]
        public void Linq_associativity()
        {
            var eq = Eq.Either(Eq.String, Eq.String);
            Func<int, Either<int, string>> incr = i => Either.Left<int, string>(i + 1);
            Func<int, Either<string, string>> str = i => Either.Left<string, string>(i.ToString(CultureInfo.InvariantCulture));
            Assert.True(
                eq.Eq(
                    Either.Left<int, string>(3).Bind(incr).Bind(str),
                    Either.Left<int, string>(3).Bind(i => incr(i).Bind(str))));
            Assert.True(
                eq.Eq(
                    Either.Right<int, string>("error").Bind(incr).Bind(str),
                    Either.Right<int, string>("error").Bind(i => incr(i).Bind(str))));
        }

        [Test]
        public void Linq_from()
        {
            var eq = Eq.Either(Eq.String, Eq.String);
            Assert.True(
                eq.Eq(
                    Either.Left<string, string>("1"),
                    from a in Either.Left<int, string>(1)
                    select a.ToString(CultureInfo.InvariantCulture)));
            Assert.True(
                eq.Eq(
                    Either.Right<string, string>("error"),
                    from a in Either.Right<int, string>("error")
                    select a.ToString(CultureInfo.InvariantCulture)));
            Assert.True(
                eq.Eq(
                    Either.Left<string, string>("3"),
                    from a in Either.Left<int, string>(1)
                    from b in Either.Left<int, string>(a + 1)
                    select (a + b).ToString(CultureInfo.InvariantCulture)));
            Assert.True(
                eq.Eq(
                    Either.Right<string, string>("error"),
                    from a in Either.Right<int, string>("error")
                    from b in Either.Left<int, string>(a + 1)
                    select (a + b).ToString(CultureInfo.InvariantCulture)));
        }

        #endregion

        [TestCase(new object[] {new [] {"a", null, "b"}}, Result = new[] {"a", "b"})]
        public string[] Lefts_returns_all_left_values_in_an_enumerable(string[] input)
        {
            var eithers = input.Select(a => a.ToEither(() => "error"));
            var lefts = eithers.Lefts();
            return lefts.ToArray();
        }
    }
}
