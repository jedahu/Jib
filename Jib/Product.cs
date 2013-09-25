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

    public struct Product<A, B, C, D>
    {
        public readonly A Get1;
        public readonly B Get2;
        public readonly C Get3;
        public readonly D Get4;

        public Product(A a, B b, C c, D d)
        {
            Get1 = a;
            Get2 = b;
            Get3 = c;
            Get4 = d;
        }

        public Product<A, B, C, D> Set1(A a) { return new Product<A, B, C, D>(a, Get2, Get3, Get4); }
        public Product<A, B, C, D> Set2(B b) { return new Product<A, B, C, D>(Get1, b, Get3, Get4); }
        public Product<A, B, C, D> Set3(C c) { return new Product<A, B, C, D>(Get1, Get2, c, Get4); }
        public Product<A, B, C, D> Set4(D d) { return new Product<A, B, C, D>(Get1, Get2, Get3, d); }
    }

    public struct Product<A, B, C, D, E>
    {
        public readonly A Get1;
        public readonly B Get2;
        public readonly C Get3;
        public readonly D Get4;
        public readonly E Get5;

        public Product(A a, B b, C c, D d, E e)
        {
            Get1 = a;
            Get2 = b;
            Get3 = c;
            Get4 = d;
            Get5 = e;
        }

        public Product<A, B, C, D, E> Set1(A a) { return new Product<A, B, C, D, E>(a, Get2, Get3, Get4, Get5); }
        public Product<A, B, C, D, E> Set2(B b) { return new Product<A, B, C, D, E>(Get1, b, Get3, Get4, Get5); }
        public Product<A, B, C, D, E> Set3(C c) { return new Product<A, B, C, D, E>(Get1, Get2, c, Get4, Get5); }
        public Product<A, B, C, D, E> Set4(D d) { return new Product<A, B, C, D, E>(Get1, Get2, Get3, d, Get5); }
        public Product<A, B, C, D, E> Set5(E e) { return new Product<A, B, C, D, E>(Get1, Get2, Get3, Get4, e); }
    }

    public static class Product
    {
        public static Product<A> Create<A>(A a) { return new Product<A>(a); }
        public static Product<A, B> Create<A, B>(A a, B b) { return new Product<A, B>(a, b); }
        public static Product<A, B, C> Create<A, B, C>(A a, B b, C c) { return new Product<A, B, C>(a, b, c); }
        public static Product<A, B, C, D> Create<A, B, C, D>(A a, B b, C c, D d) { return new Product<A, B, C, D>(a, b, c, d); }
        public static Product<A, B, C, D, E> Create<A, B, C, D, E>(A a, B b, C c, D d, E e) { return new Product<A, B, C, D, E>(a, b, c, d, e); }
    }
}