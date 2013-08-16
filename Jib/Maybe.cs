using System;
using System.Collections.Generic;

namespace Jib
{
    /// <summary>
    /// An encoding of an algebraic sum type that is either empty (Nothing) or contains a single value (Just(value)).
    /// Surprisingly Linq doesn't come with one built in.
    ///
    /// <para>Useful where one would normally check for emptiness, null, catch a related exception, or create an 'empty' object via
    /// nullary constructor.</para>
    ///
    /// <para>Construct an empty Maybe{TA} using Maybe.Nothing and a full Maybe{TA} using Maybe.Just(value). Alternatively,
    /// new Mayb{TA}() will construct an empty and new Maybe{TA}(value) will construct a full Maybe{TA}.</para>
    ///
    /// <para>Linq extension methods are implemented static Maybe class.</para>
    /// </summary>
    /// <typeparam name="TA">The type of object to store.</typeparam>
    public struct Maybe<TA>
    {
        private readonly TA value;

        private readonly bool isJust;

        /// <summary>
        /// Construct a just maybe. Use <see cref="Maybe.Just{TA}"/> instead.
        /// </summary>
        public Maybe(TA value)
        {
            isJust = true;
            this.value = value;
        }

        #region Core members
        /// All other members are derived from these three members.
        /// IsJust and IsNothing could also be derived from Fold,
        /// but they're more efficient this way.

        /// <summary>
        /// True if this Maybe contains a value.
        /// </summary>
        public bool IsJust
        {
            get { return isJust; }
        }

        /// <summary>
        /// True if this Maybe does not contain a value.
        /// </summary>
        public bool IsNothing
        {
            get { return !isJust; }
        }

        /// <summary>
        /// Fold over a Maybe, evaluating a different function for just and nothing states.
        /// </summary>
        public TX Fold<TX>(Func<TA, TX> just, Func<TX> nothing)
        {
            return IsJust ? just(value) : nothing();
        }

        #endregion

        #region Derived members
        /// All these members are implemented in terms of Fold.

        #region Convenience methods

        /// <summary>
        /// Fold over a Maybe, returning a different value for just and nothing states.
        /// </summary>
        public TX Fold<TX>(Func<TA, TX> just, TX nothing)
        {
            return Fold(just, () => nothing);
        }

        /// <summary>
        /// Get the value from this Maybe if it is just, otherwise evaluate the supplied
        /// function and return that value.
        /// </summary>
        public TA ValueOr(Func<TA> elseFunc)
        {
            return Fold(x => x, elseFunc);
        }

        /// <summary>
        /// Get the value from this Maybe if it is just, otherwise return the supplied
        /// argument.
        /// </summary>
        public TA ValueOr(TA elseValue)
        {
            return Fold(x => x, () => elseValue);
        }

        #endregion

        #region Linq methods
        /// Instance aliases for Linq methods.

        /// <summary>
        /// Map a function across this Maybe. Another name for Select.
        /// </summary>
        public Maybe<TB> Map<TB>(Func<TA, TB> mapFunc)
        {
            return Fold(a => Maybe.Just(mapFunc(a)), Maybe.Nothing<TB>);
        }

        /// <summary>
        /// Bind a function across this Maybe. Another name for SelectMany but without
        /// the numerical connotation.
        /// </summary>
        public Maybe<TB> Bind<TB>(Func<TA, Maybe<TB>> bindFunc)
        {
            return Fold(bindFunc, Maybe.Nothing<TB>);
        }

        #endregion

        #region Side effecting methods
        /// All these methods perform side effects. Beware!

        /// <summary>
        /// Fold over a Maybe, performing a different action for just and nothing states.
        /// </summary>
        public void FoldVoid(Action<TA> just, Action nothing)
        {
            Fold(a => { just(a); return 0; }, () => { nothing(); return 0; });
        }

        /// <summary>
        /// Run an action on the Maybe's value if just.
        /// </summary>
        public void JustAction(Action<TA> action)
        {
            FoldVoid(action, () => { });
        }

        /// <summary>
        /// Run an action if the Maybe is nothing.
        /// </summary>
        public void NothingAction(Action action)
        {
            FoldVoid(a => { }, action);
        }

        /// <summary>
        /// Run an action on the Maybe's value if just and if the predicate returns true.
        /// </summary>
        public void JustActionWhen(Func<TA, bool> predicate, Action<TA> action)
        {
            JustAction(a => { if (predicate(a)) action(a); });
        }

        #endregion

        #region Predicate methods
        /// Methods that return a boolean.

        /// <summary>
        /// Apply a predicate to this Maybe's value, or return false.
        /// </summary>
        public bool JustTest(Func<TA, bool> predicate)
        {
            return Fold(predicate, false);
        }

        /// <summary>
        /// Test equality (using Equals) of an object with this Maybe's value.
        /// </summary>
        public bool JustEquals(TA other)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            return JustTest(a => a == null ? other == null : a.Equals(other));
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        /// <summary>
        /// Compare Maybes safely.
        /// </summary>
        public bool TypedEquals(Maybe<TA> other)
        {
            return Fold(
                a1 => other.Fold(
                    // ReSharper disable CompareNonConstrainedGenericWithNull
                    a2 => a1 == null ? a2 == null : a1.Equals(a2),
                    // ReSharper restore CompareNonConstrainedGenericWithNull
                    false),
                other.Fold(
                    a2 => false,
                    true));
        }

        #endregion

        #region Conversion members
        /// Members that construct another data type from this Maybe.

        /// <summary>
        /// Construct a Validation from a Maybe.
        /// </summary>
        public Validation<TA, TX> Validation<TX>(Func<TX> right)
        {
            return Fold(Jib.Validation.Success<TA, TX>, Jib.Validation.Failure<TA, TX>(right()));
        }

        /// <summary>
        /// Construct a Validation from a Maybe.
        /// </summary>
        public Validation<TA, TX> Validation<TX>(TX rightValue)
        {
            return Validation(() => rightValue);
        }

        /// <summary>
        /// Construct an IEnumerable from a Maybe.
        /// </summary>
        public IEnumerable<TA> Enumerable
        {
            get { return Fold(a => new[] {a}, () => new TA[] {}); }
        }

        #endregion

        #endregion

        #region Object method implementations

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is Maybe<TA> && TypedEquals((Maybe<TA>) obj);
        }

        public override int GetHashCode()
        {
            return IsNothing ? 0 : value.GetHashCode();
        }

        public override string ToString()
        {
            return Fold(
                a => "Maybe.Just<" + typeof (TA) + ">(" + a + ")",
                () => "Maybe.Nothing<" + typeof (TA) + ">");
        }

        #endregion

        #region Overloaded operators

        public static bool operator true(Maybe<TA> maybe)
        {
            return maybe.IsJust;
        }

        public static bool operator false(Maybe<TA> maybe)
        {
            return maybe.IsNothing;
        }

        /// <summary>
        /// Return the last just Maybe or the first nothing Maybe.
        /// </summary>
        public static Maybe<TA> operator &(Maybe<TA> m1, Maybe<TA> m2)
        {
            return m1.Fold(a1 => m2, m1);
        }

        /// <summary>
        /// Return the first just Maybe or the last nothing Maybe.
        /// </summary>
        public static Maybe<TA> operator |(Maybe<TA> m1, Maybe<TA> m2)
        {
            return m1.Fold(a1 => m1, m2);
        }

        public static bool operator ==(Maybe<TA> m1, Maybe<TA> m2)
        {
            return m1.TypedEquals(m2);
        }

        public static bool operator !=(Maybe<TA> m1, Maybe<TA> m2)
        {
            return !m1.TypedEquals(m2);
        }

        public static explicit operator bool(Maybe<TA> maybe)
        {
            return maybe.IsJust;
        }

        #endregion
    }
}