namespace Jib.Instances
{
    public class MaybeZipable
    {
        private MaybeZipable() { }

        public Maybe<Pair<A, B>> Zip<A, B>(Maybe<A> maybe, Maybe<B> other)
        {
            return maybe.Cata(
                Maybe.Nothing<Pair<A, B>>,
                a => other.Cata(
                    Maybe.Nothing<Pair<A, B>>,
                    b => Maybe.Just(Pair.Create(a, b))));
        }

        public static MaybeZipable Instance = new MaybeZipable();

        public static IZipable<A> Dynamic<A>(Maybe<A> maybe)
        {
            return new DynamicZipable<A>(maybe);
        }

        private class DynamicZipable<A>
            : ZipableBase<A>
        {
            private readonly Maybe<A> maybe;

            public DynamicZipable(Maybe<A> maybe)
                : base(typeof(Maybe<A>))
            {
                this.maybe = maybe;
            }

            public override IZipable<Pair<A, B>> DoZip<B>(IZipable<B> other)
            {
                return new DynamicZipable<Pair<A, B>>(Instance.Zip(maybe, (Maybe<B>)other.Value));
            }

            public override object Value
            {
                get { return maybe; }
            }
        }
    }
}
