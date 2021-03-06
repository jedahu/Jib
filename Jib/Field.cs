﻿using System;
using Jib.Syntax;

namespace Jib
{
    public class Field<A, B>
    {
        private readonly A record;
        private readonly Func<A, B> get;
        private readonly Func<A, B, A> set;

        public Field(A record, Func<A, B> get, Func<A, B, A> set)
        {
            this.record = record;
            this.get = get;
            this.set = set;
        }

        public B Get
        {
            get { return get(record); }
        }

        public A Set(B a)
        {
            return set(record, a);
        }

        public A Update(Func<B, B> f)
        {
            return set(record, f(get(record)));
        }

        public Field<A, C> Choose<C>(Func<B, Field<B, C>> f)
        {
            var b = get(record);
            var bcf = f(get(record));
            return new Field<A, C>(
                record,
                get.Map(bcf.get),
                (a, c) => set(a, bcf.set(b, c)));
        }
    }

    public static class Field
    {
        public static Field<A, B> Create<A, B>(A record, Func<A, B> get, Func<A, B, A> set)
        {
            return new Field<A, B>(record, get, set);
        }

    }

    public static class FieldExtensions
    {
        public static Field<A, B> Field<A, B>(this A record, Func<A, B> get, Func<A, B, A> set)
        {
            return Jib.Field.Create(record, get, set);
        }
    }
}