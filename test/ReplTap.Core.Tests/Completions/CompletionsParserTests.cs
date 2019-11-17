using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class CompletionsParserTests
    {
        [Test]
        [TestCase("c", ExpectedResult = "c")]
        [TestCase(";c", ExpectedResult = "c")]
        [TestCase(";cd", ExpectedResult = "cd")]
        [TestCase(";c123", ExpectedResult = "c123")]

        [TestCase("b.c", ExpectedResult = "c")]
        [TestCase("ab.cd", ExpectedResult = "cd")]
        [TestCase("ab.Cd", ExpectedResult = "Cd")]
        [TestCase("a b.c", ExpectedResult = "c")]
        [TestCase("a b. c", ExpectedResult = "c")]
        [TestCase("a b.  c", ExpectedResult = "c")]

        [TestCase("a b,c", ExpectedResult = "c")]
        [TestCase("a b{c", ExpectedResult = "c")]
        [TestCase("a b}c", ExpectedResult = "c")]
        [TestCase("a b(c", ExpectedResult = "c")]
        [TestCase("a b)c", ExpectedResult = "c")]
        [TestCase("a b[c", ExpectedResult = "c")]
        [TestCase("a b]c", ExpectedResult = "c")]
        public string ParseIncompleteText_Should_Return_Expected(string code)
        {
            // arrange
            var parser = new CompletionsParser();

            // act
            var incompleteText = parser.ParseIncompleteText(code);

            // assert
            Assert.That(incompleteText, Is.Not.Null.And.Not.Empty);

            return incompleteText;
        }
    }
}