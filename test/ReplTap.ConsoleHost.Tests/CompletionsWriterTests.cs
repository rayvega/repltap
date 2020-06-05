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

            var variables = Enumerable
                .Range(1, 3)
                .Select(i => $"test variables {i}")
                .ToList();

            var completions = Enumerable
                .Range(1, 3)
                .Select(i => $"test completion {i}")
                .ToList();

            var expectedEmptyWriteLineCount = 1;

            var expectedCallCount = expectedEmptyWriteLineCount + variables.Count + completions.Count;

            var completionsProvider = new Mock<ICompletionsProvider>();
            completionsProvider
                .Setup(c => c.GetCompletions(code))
                .ReturnsAsync(completions);

            var variablesFilter = new Mock<IVariablesFilter>();
            variablesFilter
                .Setup(v => v.Filter(code, variables))
                .Returns(variables);

            var console = new Mock<IConsole>();

            var completionsWriter = new CompletionsWriter(completionsProvider.Object, console.Object, variablesFilter.Object);

            // act
            await completionsWriter.WriteAllCompletions(code, variables);

            // assert
            completionsProvider.Verify(c => c.GetCompletions(code), Times.Once);
            variablesFilter.Verify(c => c.Filter(code, variables), Times.Once);

            console.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(expectedCallCount));
        }
    }
}