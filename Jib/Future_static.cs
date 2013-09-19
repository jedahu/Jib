using System;

namespace Jib
{
    public static class Future
    {
        public static Future<A> Func<A>(Func<A> f, IStrategy s = null)
        {
            return new Future<A>(cb => cb(f()), s);
        }

        public static Future<A> Lazy<A>(Lazy<A> la, IStrategy s = null)
        {
            return new Future<A>(cb => cb(la.Value), s);
        }

        public static Future<A> Async<A>(Action<Action<A>> action, IStrategy s = null)
        {
            return new Future<A>(action, s);
        }

        public static Future<A> Now<A>(A a)
        {
            return new Future<A>(cb => cb(a), Strategies.Id);
        }

        public static Future<A> Strategy<A>(Future<A> fa, IStrategy s)
        {
            return new Future<A>(fa.Callback, s);
        }

        public static A CoPure<A>(this Future<A> fa)
        {
            return fa.Run();
        }

        public static Future<B> CoBind<A, B>(this Future<A> fa, Func<Future<A>, B> f)
        {
            return Now(f(fa));
        }

        public static Future<Future<A>> CoJoin<A>(this Future<A> fa)
        {
            return Now(fa);
        }
    }
}