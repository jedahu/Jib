using System;
using System.Collections.Generic;

namespace Jib
{
    /// <summary>
    /// An encoding of an algebraic sum type that contains either a 'left' or a 'right' value. Conventionally, Left is used
    /// to represent a desired value and Right an error value. Useful for validation and control flow
    /// as an alternative to exceptions.
    /// </summary>
    /// <typeparam name="TA">The type of a Left value.</typeparam>
    /// <typeparam name="TX">The type of a Right value.</typeparam>
    public struct Either<TA, TX>
    {
        private readonly TA leftValue;
        private readonly TX rightValue;
        private readonly bool isLeft;

        /// <summary>
        /// Construct an Either, which will contain a left or right value depending on
        /// the value of isLeft. Don't use this constructor. Use <see cref="Either.Left{TA,TX}"/>
        /// and <see cref="Either.Right{TA,TX}"/> instead.
        /// </summary>
        internal Either(TA leftValue, TX rightValue, bool isLeft)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            this.isLeft = isLeft;
        }

        /// <summary>
        /// Whether this Either is left (contains a value of type TA).
        /// </summary>
        public bool IsLeft
        {
            get { return isLeft; }
        }

        /// <summary>
        /// Whether this Either is right (contains a value of type TX).
        /// </summary>
        public bool IsRight
        {
            get { return !isLeft; }
        }

        /// <summary>
        /// Fold over an Either, calling a different function for left and right states.
        /// </summary>
        /// <typeparam name="TZ">The return type of the fold.</typeparam>
        /// <param name="left">The function that is called when the Either is Left.</param>
        /// <param name="right">The function that is called when the Either is Right.</param>
        /// <returns>A value of type TZ.</returns>
        public TZ Fold<TZ>(Func<TA, TZ> left, Func<TX, TZ> right)
        {
            return IsLeft ? left(leftValue) : right(rightValue);
        }

        /// <summary>
        /// Fold over an Either, performing a different action for left and right states.
        /// </summary>
        /// <param name="left">The action that is performed when the Either is Left.</param>
        /// <param name="right">The action that is performed when the Either is right.</param>
        public void FoldVoid(Action<TA> left, Action<TX> right)
        {
            Fold(a => { left(a); return 0; }, x => { right(x); return 0; });
        }

        public Validation<TA, TX> Validation
        {
            get { return Fold(Jib.Validation.Success<TA, TX>, Jib.Validation.Failure<TA, TX>); }
        }

        /// <summary>
        /// Return the Either's left value wrapped in a Maybe. Returns Maybe.Nothing if
        /// this is a right Either.
        /// </summary>
        public Maybe<TA> LeftMaybe
        {
            get { return Fold(Maybe.Just, x => Maybe.Nothing<TA>()); }
        }

        /// <summary>
        /// Return the Either's right value wrapped in a Maybe. Returns Maybe.Nothing if
        /// this is a left Either.
        /// </summary>
        public Maybe<TX> RightMaybe
        {
            get { return Fold(a => Maybe.Nothing<TX>(), Maybe.Just); }
        }

        /// <summary>
        /// Return the Either's left value in an enumerable. Returns an empty
        /// enumerable if this is a right Either.
        /// </summary>
        public IEnumerable<TA> LeftEnumerable
        {
            get { return LeftMaybe.Enumerable; }
        }

        /// <summary>
        /// Return the Either's right value in an enumerable. Returns an empty
        /// enumerable if this is a left Either.
        /// </summary>
        public IEnumerable<TX> RightEnumerable
        {
            get { return RightMaybe.Enumerable; }
        }

        /// <summary>
        /// Get the left value from this Either if it is Left, otherwise call the supplied function
        /// and return its value.
        /// </summary>
        /// <param name="elseFunc">The function that is called when the Either is Right.</param>
        /// <returns>The Either's left value, or the return value of elseFunc.</returns>
        public TA LeftOr(Func<TX, TA> elseFunc)
        {
            return Fold(a => a, elseFunc);
        }

        /// <summary>
        /// Get the left value from this Either if it is Left, otherwise return the supplied
        /// argument.
        /// </summary>
        /// <param name="elseValue">The value to return when the Either is Right.</param>
        /// <returns>The Either's left value, or elseValue.</returns>
        public TA LeftOr(TA elseValue)
        {
            return Fold(a => a, b => elseValue);
        }

        /// <summary>
        /// Return the first Either that contains a left value.
        /// </summary>
        /// <param name="other">A function returning an Either.</param>
        /// <returns>This Either (if it is left), or the result of calling other.</returns>
        public Either<TA, TX> Or(Func<Either<TA, TX>> other)
        {
            var self = this;
            return Fold(a => self, x => other());
        }

        /// <summary>
        /// Return the first Either that contains a left value.
        /// </summary>
        /// <param name="other">Another Either.</param>
        /// <returns>This Either (if it is left), or other.</returns>
        public Either<TA, TX> Or(Either<TA, TX> other)
        {
            var self = this;
            return Fold(a => self, x => other);
        }

        /// <summary>
        /// Map a function across the left of this Either. The non-extension equivalent of Select.
        /// </summary>
        public Either<TB, TX> Map<TB>(Func<TA, TB> mapFunc)
        {
            return Fold(a => Either.Left<TB, TX>(mapFunc(a)), Either.Right<TB, TX>);
        }

        /// <summary>
        /// Map a function across the right of this Either.
        /// </summary>
        public Either<TA, TY> MapRight<TY>(Func<TX, TY> mapFunc)
        {
            return Fold(Either.Left<TA, TY>, x => Either.Right<TA, TY>(mapFunc(x)));
        }

        /// <summary>
        /// Bind a function across the left of this Either. The non-extension equivalent of SelectMany.
        /// </summary>
        public Either<TB, TX> Bind<TB>(Func<TA, Either<TB, TX>> bindFunc)
        {
            return Fold(bindFunc, Either.Right<TB, TX>);
        }

        /// <summary>
        /// Bind a function across the right of this Either.
        /// </summary>
        public Either<TA, TY> BindRight<TY>(Func<TX, Either<TA, TY>> bindFunc)
        {
            return Fold(Either.Left<TA, TY>, bindFunc);
        }

        /// <summary>
        /// Run an action if the Either is left.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public void LeftAction(Action<TA> action)
        {
            FoldVoid(action, x => { });
        }

        /// <summary>
        /// Run an action if the Either is right.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public void RightAction(Action<TX> action)
        {
            FoldVoid(a => { }, action);
        }

        /// <summary>
        /// Run an action if the Either is left and a predicate returns true.
        /// </summary>
        /// <param name="predicate">The predicate to test the left value against.</param>
        /// <param name="action">The action to run.</param>
        public void LeftActionIf(Func<TA, bool> predicate, Action<TA> action)
        {
            LeftAction(a => { if (predicate(a)) action(a); });
        }

        /// <summary>
        /// Run an action if the Either is left and a predicate returns true.
        /// </summary>
        /// <param name="predicate">The predicate to test the right value against.</param>
        /// <param name="action">The action to run.</param>
        public void RightActionIf(Func<TX, bool> predicate, Action<TX> action)
        {
            RightAction(x => { if (predicate(x)) action(x); });
        }

        /// <summary>
        /// Equivalent to <code>if (test) either.LeftActionIf(a =&gt; test, action)</code>.
        /// </summary>
        public void LeftActionIf(bool test, Action<TA> action)
        {
            LeftActionIf(a => test, action);
        }

        /// <summary>
        /// Equivalent to <code>if (test) either.RightActionIf(x =&gt; test, action)</code>.
        /// </summary>
        public void RightActionIf(bool test, Action<TX> action)
        {
            RightActionIf(x => test, action);
        }

        /// <summary>
        /// Apply a predicate to this Either's left value, or return false.
        /// </summary>
        /// <returns>False if the Either is right, the return value of the predicate if it is left.</returns>
        public bool LeftTest(Func<TA, bool> predicate)
        {
            return Fold(predicate, x => false);
        }

        /// <summary>
        /// Apply a predicate to this Either's right value, or return false.
        /// </summary>
        /// <returns>False if the Either is left, the return value of the predicate if it is right.</returns>
        public bool RightTest(Func<TX, bool> predicate)
        {
            return Fold(a => false, predicate);
        }

        /// <summary>
        /// Check left value equality using Equals. (Guards internally against null argument)
        /// </summary>
        /// <returns>True if the Either is left and its value equals the argument.</returns>
        public bool LeftEquals(TA other)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            return LeftTest(a => a == null ? other == null : a.Equals(other));
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        /// <summary>
        /// Check right value equality using Equals. (Guards internally against null argument)
        /// </summary>
        /// <returns>True if the Either is right and its value equals the argument.</returns>
        public bool RightEquals(TX other)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            return RightTest(x => x == null ? other == null : x.Equals(other));
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        public bool TypedEquals(Either<TA, TX> other)
        {
            return
                // ReSharper disable CompareNonConstrainedGenericWithNull
                (IsLeft && other.IsLeft &&
                 ((leftValue != null && leftValue.Equals(other.leftValue)) ||
                  (leftValue == null && other.leftValue == null))) ||
                (IsRight && other.IsRight &&
                 ((rightValue != null && rightValue.Equals(other.rightValue)) ||
                  (rightValue == null && other.rightValue == null)));
                // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is Either<TA, TX> && TypedEquals((Either<TA, TX>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int leftPrime = 193;
                const int rightPrime = 769;
                if (IsLeft)
                {
                    // ReSharper disable RedundantCast
                    if (!typeof (TA).IsValueType && (leftValue as object) == null)
                        return leftPrime*0.GetHashCode();
                    // ReSharper restore RedundantCast
                    return leftPrime*leftValue.GetHashCode();
                }
                // ReSharper disable RedundantCast
                if (!typeof (TX).IsValueType && (rightValue as object) == null)
                    return rightPrime*0.GetHashCode();
                // ReSharper restore RedundantCast
                return rightPrime*rightValue.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Fold(
                a => "Either.Left<" + typeof (TA) + ">(" + a + ")",
                x => "Either.Right<" + typeof (TX) + ">(" + x + ")");
        }
    }
}