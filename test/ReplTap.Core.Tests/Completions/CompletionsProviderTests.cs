using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using Moq;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class CompletionsProviderTests
    {
        [Test]
        public async Task GetCompletions_Should_Return_Expected()
        {
            // arrange
            var code = "test code";

            // roslyn provider
            var roslyn = new Mock<IRoslynCompletionsProvider>();

            var completionItems = Enumerable
                .Range(1, 3)
                .Select(i => CompletionItem.Create($"displayText {i}"))
                .ToImmutableArray();

            var completionList = CompletionList.Create(TextSpan.FromBounds(0, 10), completionItems);

            roslyn
                .Setup(r => r.GetCompletions(code))
                .ReturnsAsync(completionList);

            // parser
            var parser = new Mock<ICompletionsParser>();

            var lastToken = "test-incomplete-text";

            parser
                .Setup(p => p.ParseLastToken(code))
                .Returns(lastToken);

            // filter
            var filter = new Mock<ICompletionsFilter>();

            var filtered = Enumerable
                .Range(1, 3)
                .Select(i => $"filtered item {i}");

            filter
                .Setup(f => f.Apply(It.IsAny<IEnumerable<string>>(), lastToken))
                .Returns(filtered);

            var provider = new CompletionsProvider(roslyn.Object, parser.Object, filter.Object);

            // act
            var completions = (await provider.GetCompletions(code)).ToArray();

            // assert
            Assert.That(completions, Is.Not.Null);

            roslyn.VerifyAll();
            parser.VerifyAll();
            filter.VerifyAll();
        }

        [Test]
        public async Task GetCompletions_Should_Return_Empty_List_When_No_Code()
        {
            // arrange
            var code = string.Empty;
            var provider = new CompletionsProvider(null!, null!, null!);

            // act
            var completions = (await provider.GetCompletions(code)).ToArray();

            // assert
            Assert.That(completions, Is.Empty);
        }

        [Test]
        public async Task GetCompletions_Should_Return_Empty_List_When_No_Completions()
        {
            // arrange
            var code = "test code";

            var roslyn = new Mock<IRoslynCompletionsProvider>();

            roslyn
                .Setup(r => r.GetCompletions(code))
                .ReturnsAsync((CompletionList)null);

            var provider = new CompletionsProvider(roslyn.Object, null!, null!);

            // act
            var completions = (await provider.GetCompletions(code)).ToArray();

            // assert
            Assert.That(completions, Is.Empty);
        }
    }
}