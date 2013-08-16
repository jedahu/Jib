using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Jib.Tests
{
    public class MaybeTest
    {
        #region Construction

        [Test]
        public void Maybe_constructed_with_null_is_full()
        {
            Assert.IsTrue(Maybe.Just<string>(null).IsJust);
        }

        [Test]
        public void Just_constructor_creates_full_Maybe()
        {
            Assert.IsTrue(Maybe.Just("asdf").IsJust);
            Assert.IsTrue(Maybe.Just<string>(null).IsJust);
        }

        [Test]
        public void Nothing_constructor_creates_empty_Maybe()
        {
            Assert.IsTrue(Maybe.Nothing<int>().IsNothing);
        }

        #endregion

        #region Equality

        [Test]
        public void Empty_Maybe_equals_empty_Maybe_of_same_type_parameter()
        {
            var nothing = Maybe.Nothing<int>();
            Assert.AreEqual(nothing, nothing);
            Assert.AreEqual(Maybe.Nothing<int>(), Maybe.Nothing<int>());
            Assert.AreNotEqual(Maybe.Nothing<int>(), Maybe.Nothing<string>());
        }

        [Test]
        public void Just_equals_Just()
        {
            var just = Maybe.Just(3);
            var refJust = Maybe.Just("c");
            Assert.AreEqual(just, just);
            Assert.AreEqual(refJust, refJust);
            Assert.AreEqual(Maybe.Just("a"), Maybe.Just("a"));
            Assert.AreEqual(Maybe.Just("a"), Maybe.Just("a"));
            Assert.AreNotEqual(Maybe.Just(1), Maybe.Just(2));
            Assert.AreNotEqual(Maybe.Just("a"), Maybe.Just("b"));
        }

        [Test]
        public void Nothing_and_empty_Maybe_symmetrically_dont_equal_Just()
        {
            Assert.AreNotEqual(Maybe.Nothing<int>(), Maybe.Just(3));
            Assert.AreNotEqual(Maybe.Just(3), Maybe.Nothing<int>());
            Assert.AreNotEqual(Maybe.Nothing<int>(), Maybe.Just(3));
            Assert.AreNotEqual(Maybe.Just(3), Maybe.Nothing<int>());
        }

        #endregion

        #region Core members

        [Test]
        public void Fold_on_Just_returns_just_arg_application()
        {
            Assert.AreEqual(3, Maybe.Just(2).Fold(a => a + 1, () => 8));
        }

        [Test]
        public void Fold_on_Nothing_returns_nothing_arg_evaluation()
        {
            Assert.AreEqual(8, Maybe.Nothing<int>().Fold(a => a + 1, () => 8));
        }

        [Test]
        public void Fold_on_Just_does_not_evaluate_nothing_arg()
        {
            var evaluated = false;
            Maybe.Just(2).Fold(a => a + 1, () => { evaluated = true; return 8; });
            Assert.IsFalse(evaluated);
        }

        [Test]
        public void Fold_on_Nothing_does_not_apply_just_arg()
        {
            var applied = false;
            Maybe.Nothing<int>().Fold(a => { applied = true; return a + 1; }, () => 8);
            Assert.IsFalse(applied);
        }

        #endregion

        #region Convenience methods

        [Test]
        public void Strict_Fold_is_equivalent_to_Fold()
        {
            Assert.AreEqual(Maybe.Just(2).Fold(a => a + 1, () => 8), Maybe.Just(2).Fold(a => a + 1, 8));
            Assert.AreEqual(Maybe.Nothing<int>().Fold(a => a + 1, () => 8), Maybe.Nothing<int>().Fold(a => a + 1, 8));
        }

        // ValueOr doesn't require testing. They typesystem ensures that it has only one possible implementation.
        // (Barring reflection, which isn't used.)

        #endregion

        #region Linq methods

        [Test]
        public void Linq_left_identity()
        {
            Func<string, Maybe<char>> maybeFirst = s => String.IsNullOrEmpty(s) ? Maybe.Nothing<char>() : Maybe.Just(s[0]);
            Assert.AreEqual(Maybe.Just("abc").Bind(maybeFirst), maybeFirst("abc"));
            Assert.AreEqual(Maybe.Just("").Bind(maybeFirst), maybeFirst(""));
        }

        [Test]
        public void Linq_right_identity()
        {
            Assert.AreEqual(Maybe.Just(3).Bind(Maybe.Just), Maybe.Just(3));
            Assert.AreEqual(Maybe.Nothing<int>().Bind(Maybe.Just), Maybe.Nothing<int>());
        }

        [Test]
        public void Linq_associativity()
        {
            Func<int, Maybe<int>> incr = i => Maybe.Just(i + 1);
            Func<int, Maybe<string>> str = i => Maybe.Just(i.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Maybe.Just(3).Bind(incr).Bind(str), Maybe.Just(3).Bind(i => incr(i).Bind(str)));
            Assert.AreEqual(Maybe.Nothing<int>().Bind(incr).Bind(str), Maybe.Nothing<int>().Bind(i => incr(i).Bind(str)));
        }

        [Test]
        public void Linq_from()
        {
            Assert.AreEqual(Maybe.Just("1"),
                            from x in Maybe.Just(1)
                            select x.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Maybe.Nothing<string>(),
                            from x in Maybe.Nothing<int>()
                            select x.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Maybe.Just("3"),
                            from x in Maybe.Just(1)
                            from y in Maybe.Just(x + 1)
                            select (x + y).ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void Linq_where()
        {
            Assert.AreEqual(Maybe.Just(3), from x in Maybe.Just(3) where x % 2 == 1 select x);
            Assert.AreEqual(Maybe.Nothing<int>(), from x in Maybe.Just(2) where x % 2 == 1 select x);
        }

        #endregion

        #region Side-effecting methods

        [Test]
        public void FoldVoid_calls_appropriate_actions()
        {
            var just1 = 0;
            var nothing1 = 0;
            Maybe.Just(2).FoldVoid(a => just1 = a + 1, () => nothing1 = 8);
            Assert.AreEqual(3, just1);
            Assert.AreEqual(0, nothing1);

            var just2 = 0;
            var nothing2 = 0;
            Maybe.Nothing<int>().FoldVoid(a => just2 = a + 1, () => nothing2 = 8);
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
        public void JustEquals_returns_true_only_when_Maybe_is_Just_and_value_is_equal()
        {
            Assert.IsTrue(Maybe.Just(3).JustEquals(3));
            Assert.IsFalse(Maybe.Just(3).JustEquals(2));
            Assert.IsTrue(Maybe.Just<string>(null).JustEquals(null));
            Assert.IsFalse(Maybe.Nothing<int>().JustEquals(2));
            Assert.IsFalse(Maybe.Nothing<string>().JustEquals(null));
        }

        [Test]
        public void TypedEquals_works_as_expected()
        {
            Assert.IsTrue(Maybe.Just(3).TypedEquals(Maybe.Just(3)));
            Assert.IsTrue(Maybe.Just<string>(null).TypedEquals(Maybe.Just<string>(null)));
            Assert.IsFalse(Maybe.Just(3).TypedEquals(Maybe.Just(2)));
            Assert.IsFalse(Maybe.Just("a").TypedEquals(Maybe.Just<string>(null)));
            Assert.IsFalse(Maybe.Just<string>(null).TypedEquals(Maybe.Just("a")));
            Assert.IsFalse(Maybe.Just(2).TypedEquals(Maybe.Nothing<int>()));
            Assert.IsFalse(Maybe.Nothing<int>().TypedEquals(Maybe.Just(2)));
            Assert.IsTrue(Maybe.Nothing<int>().TypedEquals(Maybe.Nothing<int>()));
        }

        #endregion

        #region Overloaded operators

        [Test]
        public void Just_is_true_and_Nothing_is_false()
        {
            Assert.IsTrue((bool) Maybe.Just(3));
            Assert.IsFalse((bool) Maybe.Nothing<int>());
        }

        [Test]
        public void Logical_and_operator()
        {
            Assert.AreEqual(Maybe.Just(3), Maybe.Just(2) && Maybe.Just(3));
            Assert.AreEqual(Maybe.Nothing<int>(), Maybe.Just(2) && Maybe.Nothing<int>());
            Assert.AreEqual(Maybe.Nothing<int>(), Maybe.Nothing<int>() && Maybe.Just(2));
            Assert.AreEqual(Maybe.Nothing<int>(), Maybe.Nothing<int>() && Maybe.Nothing<int>());
        }

        [Test]
        public void Logical_or_operator()
        {
            Assert.AreEqual(Maybe.Just(2), Maybe.Just(2) || Maybe.Just(3));
            Assert.AreEqual(Maybe.Just(2), Maybe.Just(2) || Maybe.Nothing<int>());
            Assert.AreEqual(Maybe.Just(2), Maybe.Nothing<int>() || Maybe.Just(2));
            Assert.AreEqual(Maybe.Nothing<int>(), Maybe.Nothing<int>() || Maybe.Nothing<int>());
        }

        [Test]
        public void Equality_operator()
        {
            Assert.IsTrue(Maybe.Just(2) == Maybe.Just(1 + 1));
            // ReSharper disable EqualExpressionComparison
            Assert.IsTrue(Maybe.Nothing<int>() == Maybe.Nothing<int>());
            // ReSharper restore EqualExpressionComparison
            Assert.IsFalse(Maybe.Just(2) == Maybe.Just(3));
            Assert.IsFalse(Maybe.Just(2) == Maybe.Nothing<int>());
            Assert.IsFalse(Maybe.Nothing<int>() == Maybe.Just(2));
        }

        #endregion

        #region Non-nullsafe interoperation

        [Test]
        public void Nullable_conversion_to_Maybe()
        {
            Assert.AreEqual(Maybe.Just(3), new int?(3).ToMaybe());
            Assert.AreEqual(Maybe.Nothing<int>(), new int?().ToMaybe());
        }

        [Test]
        public void Reference_conversion_to_Maybe()
        {
            Assert.AreEqual(Maybe.Just("abc"), "abc".ToMaybe());
            Assert.AreEqual(Maybe.Nothing<string>(), (null as string).ToMaybe());
        }

        [Test]
        public void MaybeFirst_returns_Just_for_non_empty_enumerable()
        {
            Assert.AreEqual(Maybe.Just(3), new[] {3, 2, 1}.MaybeFirst());
        }

        [Test]
        public void MaybeFirst_returns_Nothing_for_empty_enumerable()
        {
            Assert.AreEqual(Maybe.Nothing<int>(), new int[] {}.MaybeFirst());
        }

        [Test]
        public void MaybeFirst_does_not_catch_exception_for_null_enumerable()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>) null).MaybeFirst());
        }

        [Test]
        public void MaybeGet_returns_Just_for_valid_key()
        {
            Assert.AreEqual(Maybe.Just(3), new Dictionary<string, int> {{"foo", 3}}.MaybeGet("foo"));
        }

        [Test]
        public void MaybeGet_returns_Nothing_for_invalid_key()
        {
            Assert.AreEqual(Maybe.Nothing<int>(), new Dictionary<string, int>().MaybeGet("foo"));
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

