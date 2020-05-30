using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class CompletionsWriterTests
    {
        [Test]
        public async Task WriteAllCompletions_Should_Return_Expected()
        {
            var code = "test code";
            var variables = new List<string>();

            var completions = Enumerable
                .Range(1, 3)
                .Select(i => $"test completion {i}")
                .ToList();

            var expectedEmptyWriteLineCount = 1;

            var expectedCallCount = completions.Count + expectedEmptyWriteLineCount;

            var completionsProvider = new Mock<ICompletionsProvider>();

            completionsProvider
                .Setup(c => c.GetCompletions(code))
                .ReturnsAsync(completions);

            var console = new Mock<IConsole>();

            var completionsWriter = new CompletionsWriter(completionsProvider.Object, console.Object);

            // act
            await completionsWriter.WriteAllCompletions(code, variables);

            // assert
            completionsProvider.Verify(c => c.GetCompletions(code), Times.Once);

            console.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(expectedCallCount));
        }
    }
}