using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    [TestFixture]
    public class CompletionsCommandsTests
    {
        [Test]
        public async Task Map_Command_Should_Write_All_Completions_When_Key_TabAsync()
        {
            // arrange
            var console = new Mock<IConsole>();
            var completionsWriter = new Mock<ICompletionsWriter>();

            var text = new StringBuilder();
            text.Append("test code");

            var inputHistory = new Mock<IInputHistory>();
            inputHistory
                .Setup(i => i.AllInputsAsString())
                .Returns("all test history inputs");

            var variables = Enumerable
                .Range(1, 3)
                .Select(i => $"test variable {i}")
                .ToList();

            var state = new ConsoleState(new InputHistory())
            {
                Text = text,
                Variables = variables,
                InputHistory = inputHistory.Object,
            };

            var completionsCommands = new CompletionsCommands(console.Object, completionsWriter.Object);

            // act
            await completionsCommands.Completions(state);

            // assert
            completionsWriter.Verify(
                p => p.WriteAllCompletions($"all test history inputs{Environment.NewLine}test code", variables),
                Times.Once());
        }
    }
}
