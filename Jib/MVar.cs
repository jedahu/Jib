using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jib
{
    public struct MVar<A>
    {
        private bool empty;
        private A a;

        public static MVar<A> New(A a)
        {
            var v = new MVar<A>();
            v.empty = false;
            v.a = a;
            return v;
        }
    }
}