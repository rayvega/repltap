using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class CompletionsFilterTests
    {
        private static object[] _testCompletions =
        {
            new object[] { new[] { "abc", "mno", "abd", "klm", "acf" }, "a", new[] { "abc", "abd", "acf" } },
            new object[] { new[] { "abc", "mno", "abd", "klm" }, "ab", new[] { "abc", "abd" } },
            new object[] { new[] { "abc", "mno", "abd", "klm" }, "abc", new[] { "abc" } },

            new object[] { new[] { "abc", "mno", "abd", "klm", "acf" }, "", new[] { "abc", "mno", "abd", "klm", "acf" } },
            new object[] { new[] { "abc", "mno", "abd", "klm", "acf" }, null, new[] { "abc", "mno", "abd", "klm", "acf" } },
        };

        [Test]
        [TestCaseSource(nameof(_testCompletions))]
        public void Apply_Should_Return_Expected(string[] completionsArray, string filterText, string[] expectedCompletionsArray)
        {
            // arrange
            var completions = new List<string>(completionsArray);
            var filter = new CompletionsFilter();
            var expectedCompletions = new List<string>(expectedCompletionsArray);

            // act
            var filteredCompletions = filter.Apply(completions, filterText).ToList();

            // assert
            Assert.That(filteredCompletions, Is.Not.Null.And.Not.Empty);
            Assert.That(filteredCompletions, Is.EquivalentTo(expectedCompletions));
        }
    }
}
