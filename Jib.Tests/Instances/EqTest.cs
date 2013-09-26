using Jib.Instances;
using NUnit.Framework;

namespace Jib.Tests.Instances
{
    [TestFixture]
    public class IntEqTest
        : Generate
    {
        private readonly IEq<int> eq = PrimEq.Int;

        [TestCaseSource("Ints")]
        public void IntEq(int a)
        {
            Assert.True(eq.Eq(a, a));
        }

        [TestCaseSource("Ints")]
        public void IntNeq(int a)
        {
            Assert.False(eq.Eq(a, a + NextNat));
        }
    }

    [TestFixture]
    public class StringEqTest
        : Generate
    {
        private readonly IEq<string> eq = PrimEq.String;

        [TestCaseSource("Strings")]
        public void IntEq(string a)
        {
            Assert.True(eq.Eq(a, a));
        }

        [TestCaseSource("Strings")]
        public void IntNeq(string a)
        {
            Assert.False(eq.Eq(a, a + NextNat));
        }
    }

    [TestFixture]
    public class MaybeEqTest
        : Generate
    {
        private readonly IEq<Maybe<int>> eq = MaybeEq.Create(PrimEq.Int);

        [TestCaseSource("Ints")]
        public void SomeEq(int a)
        {
            Assert.True(eq.Eq(Maybe.Just(a), Maybe.Just(a)));
        }

        [TestCaseSource("Ints")]
        public void SomeNeq(int a)
        {
            Assert.False(eq.Eq(Maybe.Just(a), Maybe.Just(a + NextNat)));
        }

        [Test]
        public void NothingEq()
        {
            Assert.True(eq.Eq(Maybe.Nothing<int>(), Maybe.Nothing<int>()));
        }

        [TestCaseSource("Ints")]
        public void SomeNothingNeq(int a)
        {
            Assert.False(eq.Eq(Maybe.Just(a), Maybe.Nothing<int>()));
            Assert.False(eq.Eq(Maybe.Nothing<int>(), Maybe.Just(a)));
        }
    }

    [TestFixture]
    public class EitherEqTest
        : Generate
    {
        private readonly IEq<Either<string, int>> eq = EitherEq.Create(PrimEq.String, PrimEq.Int);
        private readonly IEq<Either<int, int>> ieq = EitherEq.Create(PrimEq.Int, PrimEq.Int);

        [TestCaseSource("Strings")]
        public void LeftEq(string a)
        {
            Assert.True(eq.Eq(Either.Left<string, int>(a), Either.Left<string, int>(a)));
        }

        [TestCaseSource("Strings")]
        public void LeftNeq(string a)
        {
            Assert.False(eq.Eq(Either.Left<string, int>(a), Either.Left<string, int>(a + NextNat)));
        }

        [TestCaseSource("Ints")]
        public void RightEq(int a)
        {
            Assert.True(eq.Eq(Either.Right<string, int>(a), Either.Right<string, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void RightNeq(int a)
        {
            Assert.False(eq.Eq(Either.Right<string, int>(a), Either.Right<string, int>(a + NextNat)));
        }

        [TestCaseSource("Ints")]
        public void LeftRightNeq(int a)
        {
            Assert.False(ieq.Eq(Either.Left<int, int>(a), Either.Right<int, int>(a)));
        }
    }

    [TestFixture]
    public class ValidationEqTest
        : Generate
    {
        private readonly IEq<Validation<char, int>> eq = ValidationEq.Create(PrimEq.Char, PrimEq.Int);
        private readonly IEq<Validation<int, int>> ieq = ValidationEq.Create(PrimEq.Int, PrimEq.Int);

        [TestCaseSource("NonEmptyStrings")]
        public void FailureEq(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.True(eq.Eq(Validation.Failure<char, int>(chars), Validation.Failure<char, int>(chars)));
        }

        [TestCaseSource("NonEmptyStrings")]
        public void FailureNeq(string a)
        {
            var chars = 'a'.Cons(a);
            Assert.False(eq.Eq(Validation.Failure<char, int>(chars), Validation.Failure<char, int>('a'.Cons(chars))));
        }

        [TestCaseSource("Ints")]
        public void SuccessEq(int a)
        {
            Assert.True(eq.Eq(Validation.Success<char, int>(a), Validation.Success<char, int>(a)));
        }

        [TestCaseSource("Ints")]
        public void SuccessNeq(int a)
        {
            Assert.False(eq.Eq(Validation.Success<char, int>(a), Validation.Success<char, int>(a + NextNat)));
        }

        [TestCaseSource("Ints")]
        public void FailureSuccessNeq(int a)
        {
            Assert.False(ieq.Eq(Validation.Success<int, int>(a), Validation.Failure<int, int>(a)));
            Assert.False(ieq.Eq(Validation.Failure<int, int>(a), Validation.Success<int, int>(a)));
        }
    }

    [TestFixture]
    public class NonEmptyLazyListEqTest
        : Generate
    {
        private readonly IEq<NonEmptyLazyList<int>> eq = NonEmptyLazyListEq.Create(PrimEq.Int);

        [TestCaseSource("Intss")]
        public void Eq(int[] xs)
        {
            Assert.True(eq.Eq(1.Cons(xs), 1.Cons(xs)));
        }

        [TestCaseSource("Intss")]
        public void Neq(int[] xs)
        {
            Assert.False(eq.Eq(1.Cons(xs), 2.Cons(xs)));
        }
    }
}
