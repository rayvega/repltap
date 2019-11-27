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

            var incompleteText = "test incomplete text";

            parser
                .Setup(p => p.ParseIncompleteText(code))
                .Returns(incompleteText);

            // filter
            var filter = new Mock<ICompletionsFilter>();

            var filtered = Enumerable
                .Range(1, 3)
                .Select(i => $"filtered item {i}");

            filter
                .Setup(f => f.Apply(It.IsAny<IEnumerable<string>>(), incompleteText))
                .Returns(filtered);

            var completionsProvider = new CompletionsProvider(roslyn.Object, parser.Object, filter.Object);

            // act
            var completions = (await completionsProvider.GetCompletions(code)).ToArray();

            // assert
            Assert.That(completions, Is.Not.Null);

            roslyn.VerifyAll();
            parser.VerifyAll();
            filter.VerifyAll();
        }
    }
}