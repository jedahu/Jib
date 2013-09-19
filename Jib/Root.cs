using System;

namespace Jib
{
    public static class Unwrinkle
    {
        public static bool Equals()
        {
            throw new NotSupportedException();
        }

        public static new int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public static new string ToString()
        {
            throw new NotSupportedException();
        }
    }

    public class Unobject
    {
        public override bool Equals(object other)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            throw new NotSupportedException();
        }
    }
}
