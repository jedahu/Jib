using System;

namespace Jib
{
    public struct Either<X, A>
    {
        private readonly A rightValue;
        private readonly X leftValue;
        private readonly bool isRight;

        internal Either(X leftValue, A rightValue, bool isRight)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            this.isRight = isRight;
        }

        public B Cata<B>(Func<X, B> left, Func<A, B> right)
        {
            return isRight ? right(rightValue) : left(leftValue);
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