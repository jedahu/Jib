using System;
using Jib.Instances;
using NUnit.Framework;
using Jib.Syntax;

// FUNCTOR LAWS
// identity: fmap id === id
// distribution: fmap (f . g) === fmap f . fmap g

namespace Jib.Tests.Syntax
{
    [TestFixture]
    public abstract class FunctorLaws
        : Generate
    {
        protected static readonly Func<int, int> F = x => x + 2;

        protected static readonly Func<int, int> G = x => x*3;
    }

    public class MaybeFunctorLaws
        : FunctorLaws
    {
        private readonly IEq<Maybe<int>> eq = MaybeEq.Create(PrimEq.Int);

        private readonly Func<Maybe<int>, Maybe<int>> mapId = m => m.Map(x => x);
        private readonly Func<Maybe<int>, Maybe<int>> mapFG = m => m.Map(F.Map(G));
        private readonly Func<Maybe<int>, Maybe<int>> mapFmapG = m => m.Map(G).Map(F);

        [TestCaseSource("Ints")]
        public void IdentityLawSome(int a)
        {
            Assert.True(eq.Eq(mapId(Maybe.Just(a)), Maybe.Just(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawSome(int a)
        {
            Assert.True(eq.Eq(mapFG(Maybe.Just(a)), mapFmapG(Maybe.Just(a))));
        }

        [Test]
        public void IdentityLawNothing()
        {
            Assert.True(eq.Eq(mapId(Maybe.Nothing<int>()), Maybe.Nothing<int>()));
        }

        [Test]
        public void DistributiveLawNothing()
        {
            Assert.True(eq.Eq(mapFG(Maybe.Nothing<int>()), mapFmapG(Maybe.Nothing<int>())));
        }
    }

    public class EitherFunctorLaws
        : FunctorLaws
    {
        private readonly IEq<Either<string, int>> eq = EitherEq.Create(PrimEq.String, PrimEq.Int);

        private readonly Func<Either<string, int>, Either<string, int>> mapId = m => m.Map(x => x);
        private readonly Func<Either<string, int>, Either<string, int>> mapFG = m => m.Map(x => F(G(x)));
        private readonly Func<Either<string, int>, Either<string, int>> mapFmapG = m => m.Map(G).Map(F);

        [TestCaseSource("Ints")]
        public void IdentityLawRight(int a)
        {
            Assert.True(eq.Eq(mapId(Either.Right<string, int>(a)), Either.Right<string, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawRight(int a)
        {
            Assert.True(eq.Eq(mapFG(Either.Right<string, int>(a)), mapFmapG(Either.Right<string, int>(a))));
        }

        [TestCaseSource("Strings")]
        public void IdentityLawLeft(string a)
        {
            Assert.True(eq.Eq(mapId(Either.Left<string, int>(a)), Either.Left<string, int>(a)));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLawLeft(string a)
        {
            Assert.True(eq.Eq(mapFG(Either.Left<string, int>(a)), mapFmapG(Either.Left<string, int>(a))));
        }
    }

    public class ValidationFunctorLaws
        : FunctorLaws
    {
        private readonly IEq<Validation<char, int>> eq = ValidationEq.Create(PrimEq.Char, PrimEq.Int);

        private readonly Func<Validation<char, int>, Validation<char, int>> mapId = m => m.Map(x => x);
        private readonly Func<Validation<char, int>, Validation<char, int>> mapFG = m => m.Map(x => F(G(x)));
        private readonly Func<Validation<char, int>, Validation<char, int>> mapFmapG = m => m.Map(G).Map(F);

        [TestCaseSource("Ints")]
        public void IdentityLawSuccess(int a)
        {
            Assert.True(eq.Eq(mapId(Validation.Success<char, int>(a)), Validation.Success<char, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void DistributiveLawSuccess(int a)
        {
            Assert.True(eq.Eq(mapFG(Validation.Success<char, int>(a)), mapFmapG(Validation.Success<char, int>(a))));
        }

        [TestCaseSource("Strings")]
        public void IdentityLawFailure(string a)
        {
            a.NonEmptyLazyList().CataVoid(
                () => { throw new Exception("Test is broken."); },
                chars => Assert.True(eq.Eq(mapId(Validation.Failure<char, int>(chars)), Validation.Failure<char, int>(chars))));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLawFailure(string a)
        {
            a.NonEmptyLazyList().CataVoid(
                () => { throw new Exception("Test is broken."); },
                chars => Assert.True(eq.Eq(mapFG(Validation.Failure<char, int>(chars)), mapFmapG(Validation.Failure<char, int>(chars)))));
        }
    }

    public class NonEmptyLazyListFunctorLaws
        : FunctorLaws
    {
        protected static readonly Func<char, char> F = x => Convert.ToChar(FunctorLaws.F(x));
        protected static readonly Func<char, char> G = x => Convert.ToChar(FunctorLaws.G(x));

        private readonly IEq<NonEmptyLazyList<char>> eq = NonEmptyLazyListEq.Create(PrimEq.Char);

        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapId = m => m.Map(x => x);
        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapFG = m => m.Map(x => F(G(x)));
        private readonly Func<NonEmptyLazyList<char>, NonEmptyLazyList<char>> mapFmapG = m => m.Map(G).Map(F);

        [TestCaseSource("Strings")]
        public void IdentityLaw(string a)
        {
            a.NonEmptyLazyList().CataVoid(
                () => { throw new Exception("Test is broken."); },
                chars => Assert.True(eq.Eq(mapId(chars), chars)));
        }

        [TestCaseSource("Strings")]
        public void DistributiveLaw(string a)
        {
            a.NonEmptyLazyList().CataVoid(
                () => { throw new Exception("Test is broken."); },
                chars => Assert.True(eq.Eq(mapFG(chars), mapFmapG(chars))));
        }
    }
}
