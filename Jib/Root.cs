using System;

namespace Jib
{
    public static class Unwrinkle
    {
        public static bool Equals()
        {
            throw new NotSupportedException("Use the Eq typeclass.");
        }

        public static new int GetHashCode()
        {
            throw new NotSupportedException("Use a typeclass.");
        }

        public static new string ToString()
        {
            throw new NotSupportedException("Use the Show typeclass.");
        }
    }
}
