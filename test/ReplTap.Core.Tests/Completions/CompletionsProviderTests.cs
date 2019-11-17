using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class CompletionsProviderTests
    {
        [Test]
        [Ignore("implement filtering auto completions")]
        public async Task GetCompletions_Should_Return_Expected()
        {
            // arrange
            var code = "var foo = \"bar\";foo.";
            var provider = new RoslynCompletionsProvider(); // TODO: use mock
            var completionsProvider = new CompletionsProvider(provider);

            // act
            var completions = (await completionsProvider.GetCompletions(code)).ToArray();

            // assert
            Assert.That(completions, Is.Not.Null);
            Assert.That(completions.Length, Is.GreaterThan(0));
            Assert.That(completions, Does.Contain("Split").And.Contain("StartsWith")
                .And.Not.Contain("abstract").And.Not.Contain("while"));
        }
    }
}