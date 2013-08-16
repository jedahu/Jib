using System;
using System.Collections.Generic;

namespace Jib
{
    public interface ILazyList<out T> : IEnumerable<T>
    {
    }
}