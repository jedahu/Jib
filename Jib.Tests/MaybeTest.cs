using System;
using System.Collections.Generic;
using System.Globalization;
using Jib.Extensions;
using NUnit.Framework;

namespace Jib.Tests
{
    public class MaybeTest
    {
        #region Construction

        [Test]
        public void Maybe_constructed_with_null_is_full()
        {
            Assert.IsTrue(Maybe.Just<string>(null).IsJust());
        }

        [Test]
        public void Just_constructor_creates_full_Maybe()
        {
            Assert.IsTrue(Maybe.Just("asdf").IsJust());
            Assert.IsTrue(Maybe.Just<string>(null).IsJust());
        }

        [Test]
        public void Nothing_constructor_creates_empty_Maybe()
        {
            Assert.IsTrue(Maybe.Nothing<int>().IsNothing());
        }

        #endregion

        #region Equality

        [Test]
        public void Empty_Maybe_equals_empty_Maybe_of_same_type_parameter()
        {
            var eq = Eq.Maybe(Eq.Int);
            var nothing = Maybe.Nothing<int>();
            Assert.True(eq.Eq(nothing, nothing));
            Assert.True(eq.Eq(Maybe.Nothing<int>(), Maybe.Nothing<int>()));
        }

        [TestCase(1, 1)]
        [TestCase(-3, 5)]
        public void Just_equals_Just(int i1, int i2)
        {
            var result = Eq.Maybe(Eq.Int).Eq(
                Maybe.Just(i1),
                Maybe.Just(i2));
            if (Eq.Int.Eq(i1, i2)) Assert.True(result);
            else Assert.False(result);
        }

        [TestCase("a")]
        [TestCase("")]
        [TestCase(null)]
        public void Nothing_symmetrically_does_not_equal_Just(string s)
        {
            Assert.False(
                Eq.Maybe(Eq.String).Eq(
                Maybe.Just(s),
                Maybe.Nothing<string>()));
            Assert.False(
                Eq.Maybe(Eq.String).Eq(
                Maybe.Nothing<string>(),
                Maybe.Just(s)));
        }

        #endregion

        #region Core members

        [Test]
        public void Fold_on_Just_returns_just_arg_application()
        {
            Assert.AreEqual(3, Maybe.Just(2).Cata(a => a + 1, () => 8));
        }

        [Test]
        public void Fold_on_Nothing_returns_nothing_arg_evaluation()
        {
            Assert.AreEqual(8, Maybe.Nothing<int>().Cata(a => a + 1, () => 8));
        }

        [Test]
        public void Fold_on_Just_does_not_evaluate_nothing_arg()
        {
            var evaluated = false;
            Maybe.Just(2).Cata(a => a + 1, () => { evaluated = true; return 8; });
            Assert.IsFalse(evaluated);
        }

        [Test]
        public void Fold_on_Nothing_does_not_apply_just_arg()
        {
            var applied = false;
            Maybe.Nothing<int>().Cata(a => { applied = true; return a + 1; }, () => 8);
            Assert.IsFalse(applied);
        }

        #endregion

        #region Linq methods

        [Test]
        public void Linq_left_identity()
        {
            var eq = Eq.Maybe(Eq.Char);
            Func<string, Maybe<char>> maybeFirst = s => String.IsNullOrEmpty(s) ? Maybe.Nothing<char>() : Maybe.Just(s[0]);
            Assert.True(eq.Eq(Maybe.Just("abc").Bind(maybeFirst), maybeFirst("abc")));
            Assert.True(eq.Eq(Maybe.Just("").Bind(maybeFirst), maybeFirst("")));
        }

        [Test]
        public void Linq_right_identity()
        {
            var eq = Eq.Maybe(Eq.Int);
            Assert.True(eq.Eq(Maybe.Just(3).Bind(Maybe.Just), Maybe.Just(3)));
            Assert.True(eq.Eq(Maybe.Nothing<int>().Bind(Maybe.Just), Maybe.Nothing<int>()));
        }

        [Test]
        public void Linq_associativity()
        {
            var eq = Eq.Maybe(Eq.String);
            Func<int, Maybe<int>> incr = i => Maybe.Just(i + 1);
            Func<int, Maybe<string>> str = i => Maybe.Just(i.ToString(CultureInfo.InvariantCulture));
            Assert.True(eq.Eq(Maybe.Just(3).Bind(incr).Bind(str), Maybe.Just(3).Bind(i => incr(i).Bind(str))));
            Assert.True(eq.Eq(Maybe.Nothing<int>().Bind(incr).Bind(str), Maybe.Nothing<int>().Bind(i => incr(i).Bind(str))));
        }

        [Test]
        public void Linq_from()
        {
            var eq = Eq.Maybe(Eq.String);
            Assert.True(
                eq.Eq(
                    Maybe.Just("1"),
                    from x in Maybe.Just(1)
                    select x.ToString(CultureInfo.InvariantCulture)));
            Assert.True(
                eq.Eq(
                    Maybe.Nothing<string>(),
                    from x in Maybe.Nothing<int>()
                    select x.ToString(CultureInfo.InvariantCulture)));
            Assert.True(
                eq.Eq(
                    Maybe.Just("3"),
                    from x in Maybe.Just(1)
                    from y in Maybe.Just(x + 1)
                    select (x + y).ToString(CultureInfo.InvariantCulture)));
        }

        [Test]
        public void Linq_where()
        {
            var eq = Eq.Maybe(Eq.Int);
            Assert.True(eq.Eq(Maybe.Just(3), from x in Maybe.Just(3) where x%2 == 1 select x));
            Assert.True(eq.Eq(Maybe.Nothing<int>(), from x in Maybe.Just(2) where x%2 == 1 select x));
        }

        #endregion

        #region Side-effecting methods

        [Test]
        public void FoldVoid_calls_appropriate_actions()
        {
            var just1 = 0;
            var nothing1 = 0;
            Maybe.Just(2).CataVoid(a => just1 = a + 1, () => nothing1 = 8);
            Assert.AreEqual(3, just1);
            Assert.AreEqual(0, nothing1);

            var just2 = 0;
            var nothing2 = 0;
            Maybe.Nothing<int>().CataVoid(a => just2 = a + 1, () => nothing2 = 8);
            Assert.AreEqual(0, just2);
            Assert.AreEqual(8, nothing2);
        }

        // JustAction and NothingAction are trivial uses of FoldVoid. No need to test.

        [Test]
        public void JustActionWhen_runs_action_only_when_predicate_returns_true()
        {
            var effect1 = 0;
            Maybe.Just(2).JustActionWhen(a => a%2 == 0, a => effect1 = a + 1);
            Assert.AreEqual(3, effect1);

            var effect2 = 0;
            Maybe.Just(3).JustActionWhen(a => a%2 == 0, a => effect2 = a + 1);
            Assert.AreEqual(0, effect2);

            var effect3 = 0;
            Maybe.Nothing<int>().JustActionWhen(a => a%2 == 0, a => effect3 = a + 1);
            Assert.AreEqual(0, effect3);
        }

        #endregion

        #region Predicate methods

        [Test]
        public void JustTest_returns_true_only_when_Maybe_is_Just_and_predicate_returns_true()
        {
            Assert.IsTrue(Maybe.Just(3).JustTest(a => true));
            Assert.IsFalse(Maybe.Just(3).JustTest(a => false));
            Assert.IsFalse(Maybe.Nothing<int>().JustTest(a => true));
            Assert.IsFalse(Maybe.Nothing<int>().JustTest(a => false));
        }

        [Test]
        public void JustEq_returns_true_only_when_Maybe_is_Just_and_value_is_eq()
        {
            var intEq = Eq.Struct<int>();
            var strEq = Eq.Class<string>();
            Assert.IsTrue(Maybe.Just(3).JustEq(3, intEq));
            Assert.IsFalse(Maybe.Just(3).JustEq(2, intEq));
            Assert.IsTrue(Maybe.Just<string>(null).JustEq(null, strEq));
            Assert.IsFalse(Maybe.Nothing<int>().JustEq(2, intEq));
            Assert.IsFalse(Maybe.Nothing<string>().JustEq(null, strEq));
        }

        #endregion

        #region Non-nullsafe interoperation

        [Test]
        public void Nullable_conversion_to_Maybe()
        {
            Assert.True(Eq.Maybe(Eq.Int).Eq(Maybe.Just(3), new int?(3).ToMaybe()));
            Assert.True(new int?().ToMaybe().IsNothing());
        }

        [Test]
        public void Reference_conversion_to_Maybe()
        {
            Assert.True(Eq.Maybe(Eq.String).Eq(Maybe.Just("abc"), "abc".ToMaybe()));
            Assert.True((null as string).ToMaybe().IsNothing());
        }

        [Test]
        public void MaybeFirst_returns_Just_for_non_empty_enumerable()
        {
            Assert.True(Eq.Maybe(Eq.Int).Eq(Maybe.Just(3), new[] {3, 2, 1}.MaybeFirst()));
        }

        [Test]
        public void MaybeFirst_returns_Nothing_for_empty_enumerable()
        {
            Assert.True(new int[] {}.MaybeFirst().IsNothing());
        }

        [Test]
        public void MaybeFirst_does_not_catch_exception_for_null_enumerable()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>) null).MaybeFirst());
        }

        [Test]
        public void MaybeGet_returns_Just_for_valid_key()
        {
            Assert.True(Eq.Maybe(Eq.Int).Eq(Maybe.Just(3), new Dictionary<string, int> {{"foo", 3}}.MaybeGet("foo")));
        }

        [Test]
        public void MaybeGet_returns_Nothing_for_invalid_key()
        {
            Assert.True(new Dictionary<string, int>().MaybeGet("foo").IsNothing());
        }

        [Test]
        public void MaybeGet_does_not_catches_NullReferenceException_and_rethrows_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((IDictionary<string, int>) null).MaybeGet("foo"));
        }

        #endregion

        #region Enumerable operations

        [Test]
        public void Justs_returns_correct_values()
        {
            var nothing = Maybe.Nothing<int>();
            CollectionAssert.AreEqual(new []{1, 2, 3}, new[] {Maybe.Just(1), nothing, Maybe.Just(2), nothing, Maybe.Just(3)}.Justs());
            CollectionAssert.AreEqual(new int[] {}, new[] {nothing, nothing}.Justs());
            CollectionAssert.AreEqual(new int[] {}, new Maybe<int>[] {}.Justs());
        }

        #endregion

        // Combine and Apply type signatures constrain them to one implementation. No need to test.
    }
}

