using System;

namespace Jib
{
    public struct Product<A>
    {
        public readonly A Get1;

        public Product(A a)
        {
            Get1 = a;
        }

        public Product<A> Set1(A a) { return new Product<A>(a); }
    }

    public struct Product<A, B>
    {
        public readonly A Get1;
        public readonly B Get2;

        public Product(A a, B b)
        {
            Get1 = a;
            Get2 = b;
        }

        public Product<A, B> Set1(A a) { return new Product<A, B>(a, Get2); }
        public Product<A, B> Set2(B b) { return new Product<A, B>(Get1, b); }
    }

    public struct Product<A, B, C>
    {
        public readonly A Get1;
        public readonly B Get2;
        public readonly C Get3;

        public Product(A a, B b, C c)
        {
            Get1 = a;
            Get2 = b;
            Get3 = c;
        }

        public Product<A, B, C> Set1(A a) { return new Product<A, B, C>(a, Get2, Get3); }
        public Product<A, B, C> Set2(B b) { return new Product<A, B, C>(Get1, b, Get3); }
        public Product<A, B, C> Set3(C c) { return new Product<A, B, C>(Get1, Get2, c); }
    }

    public static class Product
    {
        public static Product<A> Create<A>(A a) { return new Product<A>(a); }
        public static Product<A, B> Create<A, B>(A a, B b) { return new Product<A, B>(a, b); }
        public static Product<A, B, C> Create<A, B, C>(A a, B b, C c) { return new Product<A, B, C>(a, b, c); }
    }
}