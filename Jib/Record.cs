using System;

namespace Jib
{
    public abstract class Record<R, A>
        where R : Record<R, A>
    {
        public readonly Product<A> Product;

        protected Record(A a)
        {
            Product = Jib.Product.Create(a);
        }

        protected abstract R Create(A a);

        private R Create(Product<A> p)
        {
            return Create(p.Get1);
        }

        protected IField<A> Field1(R record) { return new Field<A>(record, p => p.Get1, (p, f) => p.Set1(f)); }

        public interface IField<F>
        {
            F Get { get; }
            R Set(F f);
        }

        public class Field<F>
            : IField<F>
        {
            private readonly R record;
            private readonly Func<Product<A>, F> get;
            private readonly Func<Product<A>, F, Product<A>> set;

            public Field(R record, Func<Product<A>, F> get, Func<Product<A>, F, Product<A>> set)
            {
                this.record = record;
                this.get = get;
                this.set = set;
            }

            public F Get { get { return get(record.Product); } }
            public R Set(F f) { return record.Create(set(record.Product, f)); }
        }
    }

    public abstract class Record<R, A, B>
        where R : Record<R, A, B>
    {
        public readonly Product<A, B> Product;

        protected Record(A a, B b)
        {
            Product = Jib.Product.Create(a, b);
        }

        protected abstract R Create(A a, B b);

        private R Create(Product<A, B> p)
        {
            return Create(p.Get1, p.Get2);
        }

        protected IField<A> Field1(R record) { return new Field<A>(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected IField<B> Field2(R record) { return new Field<B>(record, p => p.Get2, (p, f) => p.Set2(f)); }

        public interface IField<F>
        {
            F Get { get; }
            R Set(F f);
        }

        public class Field<F>
            : IField<F>
        {
            private readonly R record;
            private readonly Func<Product<A, B>, F> get;
            private readonly Func<Product<A, B>, F, Product<A, B>> set;

            public Field(R record, Func<Product<A, B>, F> get, Func<Product<A, B>, F, Product<A, B>> set)
            {
                this.record = record;
                this.get = get;
                this.set = set;
            }

            public F Get { get { return get(record.Product); } }
            public R Set(F f) { return record.Create(set(record.Product, f)); }
        }
    }

    public abstract class Record<R, A, B, C>
        where R : Record<R, A, B, C>
    {
        public readonly Product<A, B, C> Product;

        protected Record(A a, B b, C c)
        {
            Product = Jib.Product.Create(a, b, c);
        }

        protected abstract R Create(A a, B b, C c);

        private R Create(Product<A, B, C> p)
        {
            return Create(p.Get1, p.Get2, p.Get3);
        }

        protected IField<A> Field1(R record) { return new Field<A>(record, p => p.Get1, (p, f) => p.Set1(f)); }
        protected IField<B> Field2(R record) { return new Field<B>(record, p => p.Get2, (p, f) => p.Set2(f)); }
        protected IField<C> Field3(R record) { return new Field<C>(record, p => p.Get3, (p, f) => p.Set3(f)); }

        public interface IField<F>
        {
            F Get { get; }
            R Set(F f);
        }

        private class Field<F>
            : IField<F>
        {
            private readonly R record;
            private readonly Func<Product<A, B, C>, F> get;
            private readonly Func<Product<A, B, C>, F, Product<A, B, C>> set;

            public Field(R record, Func<Product<A, B, C>, F> get, Func<Product<A, B, C>, F, Product<A, B, C>> set)
            {
                this.record = record;
                this.get = get;
                this.set = set;
            }

            public F Get { get { return get(record.Product); } }
            public R Set(F f) { return record.Create(set(record.Product, f)); }
        }
    }

    public class Person
        : Record<Person, string, int>
    {
        public readonly IField<string> Name;
        public readonly IField<int> Age;

        public Person(string name, int age)
            : base(name, age)
        {
            Name = Field1(this);
            Age = Field2(this);
        }

        protected override Person Create(string name, int age)
        {
            return new Person(name, age);
        }
    }
}
