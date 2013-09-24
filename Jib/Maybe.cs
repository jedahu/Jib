using System;

namespace Jib
{
    public struct Maybe<A>
    {
        private readonly A value;

        private readonly bool isJust;

        internal Maybe(A value)
        {
            isJust = true;
            this.value = value;
        }

        public B Cata<B>(Func<B> nothing, Func<A, B> just)
        {
            return isJust ? just(value) : nothing();
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