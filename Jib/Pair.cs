namespace Jib
{
    public struct Pair<A, B>
    {
        public readonly A Fst;

        public readonly B Snd;

        public Pair(A a, B b)
        {
            Fst = a;
            Snd = b;
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

    public static class Pair
    {
        public static Pair<A, B> Create<A, B>(A a, B b)
        {
            return new Pair<A, B>(a, b);
        }
    }
}
