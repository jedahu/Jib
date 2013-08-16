using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Jib.Tests
{
    public class EitherTest
    {
        #region Construction

        [Test]
        public void Left_constructs_a_left_Either()
        {
            Assert.IsTrue(Either.Left<int, string>(3).IsLeft);
        }

        [Test]
        public void Right_constructs_a_right_Either()
        {
            Assert.IsTrue(Either.Right<int, string>("abc").IsRight);
        }

        [Test]
        public void Constructors_allow_null()
        {
            Assert.DoesNotThrow(() => Either.Left<string, string>(null));
            Assert.DoesNotThrow(() => Either.Right<int, string>(null));
        }

        #endregion

        #region Equality

        [Test]
        public void Left_equals_Left()
        {
            var left = Either.Left<int, string>(3);
            Assert.AreEqual(left, left);
            Assert.AreEqual(Either.Left<int, string>(3), Either.Left<int, string>(3));
            Assert.AreEqual(Either.Left<string, string>(null), Either.Left<string, string>(null));
            Assert.AreNotEqual(Either.Left<int, string>(3), Either.Left<int, string>(5));
        }

        [Test]
        public void Right_equals_Right()
        {
            var right = Either.Right<int, string>("abc");
            Assert.AreEqual(right, right);
            Assert.AreEqual(Either.Right<int, string>("a"), Either.Right<int, string>("a"));
            Assert.AreEqual(Either.Right<int, string>(null), Either.Right<int, string>(null));
            Assert.AreNotEqual(Either.Right<int, int>(3), Either.Right<int, int>(5));
        }

        [Test]
        public void Left_symmetrically_does_not_equal_Right()
        {
            var left = Either.Left<object, string>("a");
            var right = Either.Right<object, string>("a");
            var leftNull = Either.Left<object, string>(null);
            var rightNull = Either.Right<object, string>(null);
            Assert.AreNotEqual(left, right);
            Assert.AreNotEqual(right, left);
            Assert.AreNotEqual(leftNull, rightNull);
            Assert.AreNotEqual(rightNull, leftNull);
        }

        #endregion

        #region Fold

        [Test]
        public void Fold_on_Left_calls_left_func()
        {
            Assert.AreEqual(3, Either.Left<string, int>("abc").Fold(a => a.Length, x => x));
        }

        [Test]
        public void Fold_on_Right_calls_right_func()
        {
            Assert.AreEqual(2, Either.Right<string, int>(2).Fold(a => a.Length, x => x));
        }

        [Test]
        public void FoldVoid_on_Left_calls_left_action()
        {
            var value = -1;
            Either.Left<string, int>("abc").FoldVoid(a => value = a.Length, x => value = x);
            Assert.AreEqual(3, value);
        }

        [Test]
        public void FoldVoid_on_right_calls_right_action()
        {
            var value = -1;
            Either.Right<string, int>(2).FoldVoid(a => value = a.Length, x => value = x);
            Assert.AreEqual(2, value);
        }

        #endregion

        #region Linq

        [Test]
        public void Linq_left_identity()
        {
            Func<IList<int>, Either<int, string>> first =
                list => list.Count > 0 ?
                    Either.Left<int, string>(list[0]) :
                    Either.Right<int, string>("Empty list.");
            Assert.AreEqual(Either.Left<IList<int>, string>(new List<int> {3, 2}).Bind(first),
                            first(new List<int> {3, 2}));
            Assert.AreEqual(Either.Left<IList<int>, string>(new List<int>()).Bind(first),
                            first(new List<int>()));
        }

        [Test]
        public void Linq_right_identity()
        {
            Assert.AreEqual(Either.Left<int, string>(3).Bind(Either.Left<int, string>),
                            Either.Left<int, string>(3));
            Assert.AreEqual(Either.Right<int, string>("error").Bind(Either.Left<int, string>),
                            Either.Right<int, string>("error"));
        }

        [Test]
        public void Linq_associativity()
        {
            Func<int, Either<int, string>> incr = i => Either.Left<int, string>(i + 1);
            Func<int, Either<string, string>> str = i => Either.Left<string, string>(i.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Either.Left<int, string>(3).Bind(incr).Bind(str),
                            Either.Left<int, string>(3).Bind(i => incr(i).Bind(str)));
            Assert.AreEqual(Either.Right<int, string>("error").Bind(incr).Bind(str),
                            Either.Right<int, string>("error").Bind(i => incr(i).Bind(str)));
        }

        [Test]
        public void Linq_from()
        {
            Assert.AreEqual(Either.Left<string, string>("1"),
                            from a in Either.Left<int, string>(1)
                            select a.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Either.Right<string, string>("error"),
                            from a in Either.Right<int, string>("error")
                            select a.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Either.Left<string, string>("3"),
                            from a in Either.Left<int, string>(1)
                            from b in Either.Left<int, string>(a + 1)
                            select (a + b).ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Either.Right<string, string>("error"),
                            from a in Either.Right<int, string>("error")
                            from b in Either.Left<int, string>(a + 1)
                            select (a + b).ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        [TestCase(new object[] {new [] {"a", null, "b"}}, Result = new[] {"a", "b"})]
        public string[] Lefts_returns_all_left_values_in_an_enumerable(string[] input)
        {
            var eithers = input.Select(a => a.ToEither("error"));
            var lefts = eithers.Lefts();
            return lefts.ToArray();
        }
    }
}
