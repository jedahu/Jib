using System;
using System.Collections.Generic;
using System.Threading;

namespace Jib
{
    public class Future<A>
    {
        internal readonly Action<Action<A>> callback;

        internal readonly Strategy strategy;

        public Future(Action<Action<A>> callback, Strategy strategy = null)
        {
            this.callback = callback;
            this.strategy = strategy ?? Strategies.Default;
        }

        public void RunAsync(Action<A> action)
        {
            strategy.Call(() => callback(action));
        }

        public A Run()
        {
            var latch = new CountdownEvent(1);
            var call = strategy.Call(() =>
                {
                    A result = default(A);
                    callback(a =>
                        {
                            result = a;
                            latch.Signal();
                        });
                    latch.Wait();
                    return result;
                });
            return call();
        }

        public Future<Pair<A, B>> Zip<B>(Future<B> fb)
        {
            return Future.Async<Pair<A, B>>(cb => callback(a => fb.callback(b => cb(new Pair<A, B>(a, b)))), fb.strategy);
        }

        public Future<Pair<A, B>> PZip<B>(Future<B> fb)
        {
            return Future.Async<Pair<A, B>>(cb =>
                {
                    var latch = new CountdownEvent(2);
                    A resA = default(A);
                    B resB = default(B);
                    Action next = () => cb(new Pair<A, B>(resA, resB));
                    RunAsync(a =>
                        {
                            resA = a;
                            latch.Signal();
                            latch.Wait();
                            next();
                        });
                    fb.callback(b =>
                        {
                            resB = b;
                            latch.Signal();
                        });
                },
                fb.strategy);
        }
    }


    public static class Future
    {
        public static Future<A> Func<A>(Func<A> f, Strategy s = null)
        {
            return new Future<A>(cb => cb(f()), s);
        }

        public static Future<A> Lazy<A>(Lazy<A> la, Strategy s = null)
        {
            return new Future<A>(cb => cb(la.Value), s);
        }

        public static Future<A> Async<A>(Action<Action<A>> action, Strategy s = null)
        {
            return new Future<A>(action, s);
        }

        public static Future<A> Point<A>(A a)
        {
            return new Future<A>(cb => cb(a), Strategies.Id);
        }

        public static Future<A> Strategy<A>(Future<A> fa, Strategy s)
        {
            return new Future<A>(fa.callback, s);
        }

        // Functor
        public static Future<B> Map<A, B>(this Future<A> fa, Func<A, B> f)
        {
            return new Future<B>(cb => fa.callback(a => cb(f(a))));
        }

        // Applicative
        public static Future<X> Apply<A, X>(this Future<Func<A, X>> ff, Future<A> fa)
        {
            return ff.Zip(fa).Map(p => p.Fst(p.Snd));
        }

        public static Future<X> Apply<A, B, X>(this Future<Func<A, B, X>> ff, Future<A> fa, Future<B> fb)
        {
            return ff.Zip(fa.Zip(fb)).Map(p => p.Fst.UnCurry()(p.Snd));
        }

        public static Future<X> Apply<A, B, C, X>(this Future<Func<A, B, C, X>> ff, Future<A> fa, Future<B> fb,
                                                  Future<C> fc)
        {
            return ff.Zip(fa.Zip(fb.Zip(fc))).Map(p => p.Fst.UnCurry()(p.Snd));
        }

        public static Future<X> PApply<A, B, X>(this Future<Func<A, B, X>> ff, Future<A> fa, Future<B> fb)
        {
            return ff.Zip(fa.PZip(fb)).Map(p => p.Fst.UnCurry()(p.Snd));
        }

        public static Future<X> PApply<A, B, C, X>(this Future<Func<A, B, C, X>> ff, Future<A> fa, Future<B> fb,
                                                  Future<C> fc)
        {
            return ff.Zip(fa.PZip(fb.PZip(fc))).Map(p => p.Fst.UnCurry()(p.Snd));
        }

        // Monad
        public static Future<B> Bind<A, B>(this Future<A> fa, Func<A, Future<B>> k)
        {
            return new Future<B>(cb => fa.callback(a => k(a).callback(cb)));
        }

        public static Future<A> Join<A>(this Future<Future<A>> m, Strategy s = null)
        {
            return Future.Async<A>(cb => m.callback(fa => fa.callback(cb)), s ?? m.strategy);
        }

        // Comonad
        public static A CoPoint<A>(this Future<A> fa)
        {
            return fa.Run();
        }

        public static Future<B> CoBind<A, B>(this Future<A> fa, Func<Future<A>, B> f)
        {
            return Future.Point(f(fa));
        }

        public static Future<Future<A>> CoJoin<A>(this Future<A> fa)
        {
            return Future.Point(fa);
        }

        // Linq
        public static Future<B> Select<A, B>(this Future<A> m, Func<A, B> f)
        {
            return m.Map(f);
        }

        public static Future<B> SelectMany<A, B>(this Future<A> m, Func<A, Future<B>> k)
        {
            return m.Bind(k);
        }

        public static Future<C> SelectMany<A, B, C>(this Future<A> m, Func<A, Future<B>> k, Func<A, B, C> f)
        {
            return m.Bind(a => k(a).Bind<B, C>(b => Point(f(a, b))));
        }
    }
}
