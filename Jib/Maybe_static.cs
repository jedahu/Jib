using System;
using System.Collections.Generic;
using System.Linq;

namespace Jib
{
    public static class Maybe
    {
        #region Constructors

        /// <summary>
        /// Construct a full Maybe.
        /// </summary>
        /// <typeparam name="TA">The type of object to store.</typeparam>
        /// <param name="value">The object to store.</param>
        /// <returns>A Maybe containing value.</returns>
        public static Maybe<TA> Just<TA>(TA value)
        {
            return new Maybe<TA>(value);
        }

        /// <summary>
        /// Construct an empty Maybe.
        /// </summary>
        /// <typeparam name="TA">The type of object a corresponding full Maybe would store.</typeparam>
        /// <returns>An empty Maybe.</returns>
        public static Maybe<TA> Nothing<TA>()
        {
            return new Maybe<TA>();
        }

        #endregion

        #region Methods for interoperating with non-nullsafe code

        /// <summary>
        /// Wrap a possibly null value in a Maybe.
        /// </summary>
        /// <typeparam name="TA">The type of the value to wrap.</typeparam>
        /// <param name="value">The value to wrap.</param>
        /// <returns>Just or Nothing.</returns>
        public static Maybe<TA> ToMaybe<TA>(this TA value) where TA : class
        {
            return value == null ? Nothing<TA>() : Just(value);
        }

        /// <summary>
        /// Wrap a possibly null value in a Maybe.
        /// </summary>
        /// <typeparam name="TA">The type of the value to wrap.</typeparam>
        /// <param name="value">The value to wrap.</param>
        /// <returns>Just or Nothing.</returns>
        public static Maybe<TA> ToMaybe<TA>(this TA? value) where TA : struct
        {
            return value.HasValue ? Just(value.Value) : Nothing<TA>();
        }

        /// <summary>
        /// Return the first value from an enumerable wrapped in a Maybe. If the enumerable is empty the returned Maybe will be empty.
        /// </summary>
        public static Maybe<TA> MaybeFirst<TA>(this IEnumerable<TA> list)
        {
            // Cannot test an IEnumerable for emptiness without 'using' it. try-catch is the only way.
            try
            {
                return Just(list.First());
            }
            catch (InvalidOperationException)
            {
                return Nothing<TA>();
            }
        }

        /// <summary>
        /// Get a value from a dictionary wrapped in a Maybe. If the value is not in the dictionary the returned Maybe will be empty.
        /// </summary>
        public static Maybe<TA> MaybeGet<TK, TA>(this IDictionary<TK, TA> dict, TK key)
        {
            try
            {
                return Just(dict[key]);
            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("MaybeGet on a null IDictionary.", e);
            }
            catch (KeyNotFoundException)
            {
                return Nothing<TA>();
            }
        }

        #endregion

        #region Enumerable operations

        /// <summary>
        /// Take an enumerable of Maybe{TA}and return an enumerable of TA.
        /// </summary>
        public static IEnumerable<TA> Justs<TA>(this IEnumerable<Maybe<TA>> maybes)
        {
            return maybes.SelectMany(m => m.Enumerable);
        }

        #endregion

        #region Combining Maybes into tuples
        /// Useful with Arity.Uncurry() and Linq.
        /// Instead of this:
        ///     var result = from a in ma from b in mb select func(a, b);
        /// 
        /// you can write this:
        ///     var result = Maybe.Combine(a, b).Map(func.Uncurry());

        /// <summary>
        /// Combine two Maybes into a Maybe{Tuple{,}}
        /// </summary>
        public static Maybe<Tuple<TA, TB>> Combine<TA, TB>(Maybe<TA> ma, Maybe<TB> mb)
        {
            return from a in ma
                   from b in mb
                   select Tuple.Create(a, b);
        }

        /// <summary>
        /// Combine three Maybes into a Maybe{Tuple{,,}}
        /// </summary>
        public static Maybe<Tuple<TA, TB, TC>> Combine<TA, TB, TC>(Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc)
        {
            return from a in ma
                   from b in mb
                   from c in mc
                   select Tuple.Create(a, b, c);
        }

        /// <summary>
        /// Combine four Maybes into a Maybe{Tuple{,,,}}
        /// </summary>
        public static Maybe<Tuple<TA, TB, TC, TD>> Combine<TA, TB, TC, TD>(Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md)
        {
            return from a in ma
                   from b in mb
                   from c in mc
                   from d in md
                   select Tuple.Create(a, b, c, d);
        }

        /// <summary>
        /// Combine five Maybes into a Maybe{Tuple{,,,,}}
        /// </summary>
        public static Maybe<Tuple<TA, TB, TC, TD, TE>> Combine<TA, TB, TC, TD, TE>(Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me)
        {
            return from a in ma
                   from b in mb
                   from c in mc
                   from d in md
                   from e in me
                   select Tuple.Create(a, b, c, d, e);
        }

        #endregion

        #region Applying functions to Maybe values
        /// Apply a function to n Maybe values but only if the values are ael just.

        /// <summary>
        /// Apply a function to one Maybe value.
        /// </summary>
        public static Maybe<TR> Apply<TA, TR>(this Func<TA, TR> func, Maybe<TA> arg)
        {
            return arg.Map(func);
        }

        /// <summary>
        /// Apply a function to two Maybe values.
        /// </summary>
        public static Maybe<TR> Apply<TA, TB, TR>(this Func<TA, TB, TR> func, Maybe<TA> arg1, Maybe<TB> arg2)
        {
            return Combine(arg1, arg2).Map(func.Uncurry());
        }

        /// <summary>
        /// Apply a function to three Maybe values.
        /// </summary>
        public static Maybe<TR> Apply<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> func, Maybe<TA> arg1, Maybe<TB> arg2, Maybe<TC> arg3)
        {
            return Combine(arg1, arg2, arg3).Map(func.Uncurry());
        }

        /// <summary>
        /// Apply a function to four Maybe values.
        /// </summary>
        public static Maybe<TR> Apply<TA, TB, TC, TD, TR>(this Func<TA, TB, TC, TD, TR> func, Maybe<TA> arg1, Maybe<TB> arg2, Maybe<TC> arg3, Maybe<TD> arg4)
        {
            return Combine(arg1, arg2, arg3, arg4).Map(func.Uncurry());
        }

        /// <summary>
        /// Apply a function to five Maybe values.
        /// </summary>
        public static Maybe<TR> Apply<TA, TB, TC, TD, TE, TR>(this Func<TA, TB, TC, TD, TE, TR> func, Maybe<TA> arg1, Maybe<TB> arg2, Maybe<TC> arg3, Maybe<TD> arg4, Maybe<TE> arg5)
        {
            return Combine(arg1, arg2, arg3, arg4, arg5).Map(func.Uncurry());
        }

        #endregion

        #region Linq methods

        public static Maybe<TB> Select<TA, TB>(this Maybe<TA> maybe, Func<TA, TB> selectFunc)
        {
            return maybe.Map(selectFunc);
        }

        public static Maybe<TB> SelectMany<TA, TB>(this Maybe<TA> maybe, Func<TA, Maybe<TB>> selectManyFunc)
        {
            return maybe.Bind(selectManyFunc);
        }

        public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> maybe, Func<TA, Maybe<TB>> selectManyFunc, Func<TA, TB, TC> combineFunc)
        {
            return maybe.Bind(a => selectManyFunc(a).Bind(b => Just(combineFunc(a, b))));
        }

        public static Maybe<TA> Where<TA>(this Maybe<TA> maybe, Func<TA, bool> predicate)
        {
            return maybe.Fold(a => predicate(a) ? maybe : Nothing<TA>(), Nothing<TA>);
        }

        #endregion
    }
}