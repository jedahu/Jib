using System;
using System.Linq;
using NUnit.Framework;

namespace Jib.Test
{
    public static class JibAssert
    {
        public static void True(params Func<bool>[] checks)
        {
            if (checks.Any(c => !c()))
            {
                throw new AssertionException("Check failed.");
            }
        }

        public static EqAssert<A> Eq<A>(IEq<A> eq, IShow<A> show)
        {
            return new EqAssert<A>(eq, show);
        }

        public static EqAssert<A> Eq<A>(IEq<A> eq)
        {
            return new EqAssert<A>(eq);
        }

        public class EqAssert<A>
        {
            private readonly IEq<A> eq;
            private readonly Maybe<IShow<A>> show;

            public EqAssert(IEq<A> eq, IShow<A> show)
            {
                this.eq = eq;
                this.show = Maybe.Just(show);
            }

            public EqAssert(IEq<A> eq)
            {
                this.eq = eq;
                show = Maybe.Nothing<IShow<A>>();
            }

            public void Check(A a1, A a2)
            {
                if (!eq.Eq(a1, a2))
                {
                    throw new AssertionException(
                        show.Cata(
                            s => string.Format("Not Eq. {0}, {1}", s.Show(a1), s.Show(a2)),
                            () => "Not Eq."));
                }
            }
        }
    }
}
