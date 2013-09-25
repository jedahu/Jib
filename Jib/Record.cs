using System;

namespace Jib
{
    public abstract class RecordBase<R, A>
        where R : RecordBase<R, A>
    {
        public readonly A Peer;

        protected RecordBase(A a)
        {
            Peer = a;
        }

        protected abstract R Create(A a);

        protected Lens<R, F> GetLens<F>(R record, Func<A, F> get, Func<A, F, A> set)
        {
            return record.Lens(r => get(r.Peer), (r, f) => Create(set(r.Peer, f)));
        }
    }

    public abstract class Record<R, A>
        : RecordBase<R, Product<A>>
        where R : Record<R, A>
    {
        protected Record(Product<A> a) : base(a) { }

        protected Lens<R, A> Lens1(R record) { return GetLens(record, p => p.Get1, (p, f) => p.Set1(f)); }
    }

    public abstract class Record<R, A, B>
        : RecordBase<R, Product<A, B>>
        where R : Record<R, A, B>
    {
        protected Record(Product<A, B> a) : base(a) { }

        protected Lens<R, A> Lens1(R record) { return GetLens(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Lens<R, B> Lens2(R record) { return GetLens(record, p => p.Get2, (p, f) => p.Set2(f)); }
    }

    public abstract class Record<R, A, B, C>
        : RecordBase<R, Product<A, B, C>>
        where R : Record<R, A, B, C>
    {
        protected Record(Product<A, B, C> a) : base(a) { }

        protected Lens<R, A> Lens1(R record) { return GetLens(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Lens<R, B> Lens2(R record) { return GetLens(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Lens<R, C> Lens3(R record) { return GetLens(record, p => p.Get3, (p, f) => p.Set3(f)); }
    }

    public abstract class Record<R, A, B, C, D>
        : RecordBase<R, Product<A, B, C, D>>
        where R : Record<R, A, B, C, D>
    {
        protected Record(Product<A, B, C, D> a) : base(a) { }

        protected Lens<R, A> Lens1(R record) { return GetLens(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Lens<R, B> Lens2(R record) { return GetLens(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Lens<R, C> Lens3(R record) { return GetLens(record, p => p.Get3, (p, f) => p.Set3(f)); }
        protected Lens<R, D> Lens4(R record) { return GetLens(record, p => p.Get4, (p, f) => p.Set4(f)); }
    }

    public abstract class Record<R, A, B, C, D, E>
        : RecordBase<R, Product<A, B, C, D, E>>
        where R : Record<R, A, B, C, D, E>
    {
        protected Record(Product<A, B, C, D, E> a) : base(a) { }

        protected Lens<R, A> Lens1(R record) { return GetLens(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Lens<R, B> Lens2(R record) { return GetLens(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Lens<R, C> Lens3(R record) { return GetLens(record, p => p.Get3, (p, f) => p.Set3(f)); }
        protected Lens<R, D> Lens4(R record) { return GetLens(record, p => p.Get4, (p, f) => p.Set4(f)); }
        protected Lens<R, E> Lens5(R record) { return GetLens(record, p => p.Get5, (p, f) => p.Set5(f)); }
    }
}
