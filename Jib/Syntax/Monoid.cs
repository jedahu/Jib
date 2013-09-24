namespace Jib.Syntax
{
    public static class MaybeMonoid
    {
        public static Maybe<A> Empty<A>()
        {
            return Monoid.Maybe<A>().Zero;
        }
    }
}
