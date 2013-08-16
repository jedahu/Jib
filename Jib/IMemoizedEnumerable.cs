using System.Collections.Generic;

namespace Jib
{
    /// <summary>
    /// This interface is only here to preserve covariance of type parameter T.
    /// See <see cref="MemoizedEnumerable"/> for relevant documentation.
    /// </summary>
    public interface IMemoizedEnumerable<out T> : IEnumerable<T>
    {
        IEnumerable<T> AsEnumerable { get; }
    }
}