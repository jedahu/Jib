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

        protected Field<R, F> GetField<F>(R record, Func<A, F> get, Func<A, F, A> set)
        {
            return record.Field(r => get(r.Peer), (r, f) => Create(set(r.Peer, f)));
        }
    }

    public abstract class Record<R, A>
        : RecordBase<R, Product<A>>
        where R : Record<R, A>
    {
        protected Record(Product<A> a) : base(a) { }

        protected Field<R, A> Field1(R record) { return GetField(record, p => p.Get1, (p, f) => p.Set1(f)); }
    }

    public abstract class Record<R, A, B>
        : RecordBase<R, Product<A, B>>
        where R : Record<R, A, B>
    {
        protected Record(Product<A, B> a) : base(a) { }

        protected Field<R, A> Field1(R record) { return GetField(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Field<R, B> Field2(R record) { return GetField(record, p => p.Get2, (p, f) => p.Set2(f)); }
    }

    public abstract class Record<R, A, B, C>
        : RecordBase<R, Product<A, B, C>>
        where R : Record<R, A, B, C>
    {
        protected Record(Product<A, B, C> a) : base(a) { }

        protected Field<R, A> Field1(R record) { return GetField(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Field<R, B> Field2(R record) { return GetField(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Field<R, C> Field3(R record) { return GetField(record, p => p.Get3, (p, f) => p.Set3(f)); }
    }

    public abstract class Record<R, A, B, C, D>
        : RecordBase<R, Product<A, B, C, D>>
        where R : Record<R, A, B, C, D>
    {
        protected Record(Product<A, B, C, D> a) : base(a) { }

        protected Field<R, A> Field1(R record) { return GetField(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Field<R, B> Field2(R record) { return GetField(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Field<R, C> Field3(R record) { return GetField(record, p => p.Get3, (p, f) => p.Set3(f)); }
        protected Field<R, D> Field4(R record) { return GetField(record, p => p.Get4, (p, f) => p.Set4(f)); }
    }

    public abstract class Record<R, A, B, C, D, E>
        : RecordBase<R, Product<A, B, C, D, E>>
        where R : Record<R, A, B, C, D, E>
    {
        protected Record(Product<A, B, C, D, E> a) : base(a) { }

        protected Field<R, A> Field1(R record) { return GetField(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected Field<R, B> Field2(R record) { return GetField(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected Field<R, C> Field3(R record) { return GetField(record, p => p.Get3, (p, f) => p.Set3(f)); }
        protected Field<R, D> Field4(R record) { return GetField(record, p => p.Get4, (p, f) => p.Set4(f)); }
        protected Field<R, E> Field5(R record) { return GetField(record, p => p.Get5, (p, f) => p.Set5(f)); }
    }
}
