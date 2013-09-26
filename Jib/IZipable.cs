using System;

namespace Jib
{
    public interface IZipable<A>
    {
        IZipable<Pair<A, B>> Zip<B>(IZipable<B> other);
        object Value { get; }
    }

    public abstract class ZipableBase<A>
        : IZipable<A>
    {
        private readonly Type valueType;

        protected ZipableBase(Type valueType)
        {
            this.valueType = valueType;
        }

        public abstract IZipable<Pair<A, B>> DoZip<B>(IZipable<B> other);
        public abstract object Value { get; }

        public IZipable<Pair<A, B>> Zip<B>(IZipable<B> other)
        {
            if (other.Value.GetType().IsInstanceOfType(valueType))
                throw new Exception();
            return DoZip(other);
        }
    }
}
