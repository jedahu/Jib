using System;
using Jib.Impure;
using Jib.Instances;
using NUnit.Framework;

// FUNCTOR LAWS
// identity: fmap id === id
// distribution: fmap (f . g) === fmap f . fmap g

namespace Jib.Tests.Instances
{
    [TestFixture]
    public abstract class FunctorLaws
        : Generate
    {
        protected static readonly Func<int, int> F = x => x + 2;
        protected static readonly Func<int, int> G = x => x*3;
        protected static readonly Func<int, int> FG = FuncFunctor.Instance.Map(G, F);
    }

    public class MaybeFunctorLaws
        : FunctorLaws
    {
        private static readonly IEq<Maybe<int>> Eq = MaybeEq.Create(PrimEq.Int);
        private static readonly MaybeFunctor M = MaybeFunctor.Instance;

        private readonly Func<Maybe<int>, Maybe<int>> mapId = m => M.Map(m, a => a);
        private readonly Func<Maybe<int>, Maybe<int>> mapFG = m => M.Map(m, FG);
        private readonly Func<Maybe<int>, Maybe<int>> mapFmapG = m => M.Map(M.Map(m, G), F);

        [TestCaseSource("Ints")]
        public void IdentityLawSome(int a)
        {
            Assert.True(Eq.Eq(mapId(Maybe.Just(a)), Maybe.Just(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawSome(int a)
        {
            Assert.True(Eq.Eq(mapFG(Maybe.Just(a)), mapFmapG(Maybe.Just(a))));
        }

        [Test]
        public void IdentityLawNothing()
        {
            Assert.True(Eq.Eq(mapId(Maybe.Nothing<int>()), Maybe.Nothing<int>()));
        }

        [Test]
        public void DistributiveLawNothing()
        {
            Assert.True(Eq.Eq(mapFG(Maybe.Nothing<int>()), mapFmapG(Maybe.Nothing<int>())));
        }
    }

    public class EitherFunctorLaws
        : FunctorLaws
    {
        private static readonly IEq<Either<string, int>> Eq = EitherEq.Create(PrimEq.String, PrimEq.Int);
        private static readonly EitherFunctor M = EitherFunctor.Instance;

        private readonly Func<Either<string, int>, Either<string, int>> mapId = m => M.Map(m, a => a);
        private readonly Func<Either<string, int>, Either<string, int>> mapFG = m => M.Map(m, FG);
        private readonly Func<Either<string, int>, Either<string, int>> mapFmapG = m => M.Map(M.Map(m, G), F);

        [TestCaseSource("Ints")]
        public void IdentityLawRight(int a)
        {
            Assert.True(Eq.Eq(mapId(Either.Right<string, int>(a)), Either.Right<string, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawRight(int a)
        {
            Assert.True(Eq.Eq(mapFG(Either.Right<string, int>(a)), mapFmapG(Either.Right<string, int>(a))));
        }

        [TestCaseSource("Strings")]
        public void IdentityLawLeft(string a)
        {
            Assert.True(Eq.Eq(mapId(Either.Left<string, int>(a)), Either.Left<string, int>(a)));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLawLeft(string a)
        {
            Assert.True(Eq.Eq(mapFG(Either.Left<string, int>(a)), mapFmapG(Either.Left<string, int>(a))));
        }
    }

    public class ValidationFunctorLaws
        : FunctorLaws
    {
        private static readonly IEq<Validation<char, int>> Eq = ValidationEq.Create(PrimEq.Char, PrimEq.Int);
        private static readonly ValidationFunctor M = ValidationFunctor.Instance;

        private readonly Func<Validation<char, int>, Validation<char, int>> mapId = m => M.Map(m, a => a);
        private readonly Func<Validation<char, int>, Validation<char, int>> mapFG = m => M.Map(m, FG);
        private readonly Func<Validation<char, int>, Validation<char, int>> mapFmapG = m => M.Map(M.Map(m, G), F);

        [TestCaseSource("Ints")]
        public void IdentityLawSuccess(int a)
        {
            Assert.True(Eq.Eq(mapId(Validation.Success<char, int>(a)), Validation.Success<char, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawSuccess(int a)
        {
            Assert.True(Eq.Eq(mapFG(Validation.Success<char, int>(a)), mapFmapG(Validation.Success<char, int>(a))));
        }

        [TestCaseSource("Strings")]
        public void IdentityLawFailure(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.True(Eq.Eq(mapId(Validation.Failure<char, int>(chars)), Validation.Failure<char, int>(chars)));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLawFailure(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.True(Eq.Eq(mapFG(Validation.Failure<char, int>(chars)), mapFmapG(Validation.Failure<char, int>(chars))));
        }
    }

    public class NonEmptyLazyListFunctorLaws
        : FunctorLaws
    {
        private static readonly IEq<NonEmptyLazyList<char>> Eq = NonEmptyLazyListEq.Create(PrimEq.Char);
        private static readonly NonEmptyLazyListFunctor M = NonEmptyLazyListFunctor.Instance;

        protected static readonly new Func<char, char> F = x => Convert.ToChar(x + 2);
        protected static readonly new Func<char, char> G = x => Convert.ToChar(x*3);
        protected static readonly new Func<char, char> FG = FuncFunctor.Instance.Map(G, F);

        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapId = m => M.Map(m, a => a);
        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapFG = m => M.Map(m, FG);
        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapFmapG = m => M.Map(M.Map(m, G), F);

        [TestCaseSource("Strings")]
        public void IdentityLaw(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.True(Eq.Eq(mapId(chars), chars));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLaw(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.True(Eq.Eq(mapFG(chars), mapFmapG(chars)));
        }
    }
}
