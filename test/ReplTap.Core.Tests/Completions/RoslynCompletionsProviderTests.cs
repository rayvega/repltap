using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class RoslynCompletionsProviderTests
    {
        [Test]
        [TestCase("var foo = \"bar\"; foo.", new[] {"Contains", "EndsWith", "Substring", "ToLower", "Trim",})]
        [TestCase("\"bar\".", new[] {"Contains", "EndsWith", "Substring", "ToLower", "Trim",})]
        public async Task GetCompletions_Should_Return_Expected_When_Dot_Completion(string code, string[] expectedCompletions)
        {
            // arrange
            var completionsProvider = new RoslynCompletionsProvider();

            // act
            var completions = await completionsProvider.GetCompletions(code);

            // assert
            var actual = completions.Items.Select(i => i.DisplayText);

            Assert.That(actual, Is.SupersetOf(expectedCompletions));
        }
    }
}