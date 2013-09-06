using System;

namespace Jib
{
    public struct Either<A, X>
    {
        private readonly A leftValue;
        private readonly X rightValue;
        private readonly bool isLeft;

        internal Either(A leftValue, X rightValue, bool isLeft)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            this.isLeft = isLeft;
        }

        public B Cata<B>(Func<A, B> left, Func<X, B> right)
        {
            return isLeft ? left(leftValue) : right(rightValue);
        }

        public override bool Equals(object obj)
        {
            return Unwrinkle.Equals();
        }

        public override int GetHashCode()
        {
            return Unwrinkle.GetHashCode();
        }

        public override string ToString()
        {
            return Unwrinkle.ToString();
        }
    }
}