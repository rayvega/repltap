using System;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class ConsoleKeyCommandsTests
    {
        [Test]
        public void Map_Command_Should_Write_All_Completions_When_Key_Tab()
        {
            // arrange
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

            var parameters = new CommandParameters
            {
                Text = text,
                Variables = variables,
                InputHistory = inputHistory.Object,
            };

            var consoleKeyCommands = new ConsoleKeyCommands(completionsWriter.Object);
            var key = (ConsoleKey.Tab, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(parameters);

            // assert
            completionsWriter.Verify(
                p => p.WriteAllCompletions($"all test history inputs{Environment.NewLine}test code", variables),
                Times.Once());
        }

        [Test]
        public void Map_Command_Should_Return_Smaller_Input_When_Key_Backspace()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var parameters = new CommandParameters
            {
                Text = text,
                Variables = null,
                InputHistory = null,
            };

            var completionsWriter = new Mock<ICompletionsWriter>();
            var consoleKeyCommands = new ConsoleKeyCommands(completionsWriter.Object);

            var key = (ConsoleKey.Backspace, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(parameters);

            // assert
            Assert.That(parameters.Text.ToString(), Is.EqualTo("test cod"));
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Up_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetPreviousInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var parameters = new CommandParameters
            {
                Text = text,
                Variables = null,
                InputHistory = inputHistory.Object,
            };

            var consoleKeyCommands = new ConsoleKeyCommands(null!);
            var key = (ConsoleKey.UpArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(parameters);

            // assert
            Assert.That(parameters.Text.ToString(), Is.EqualTo(expectedInputHistory));
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Down_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetNextInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var parameters = new CommandParameters
            {
                Text = text,
                Variables = null,
                InputHistory = inputHistory.Object,
            };

            var consoleKeyCommands = new ConsoleKeyCommands(null!);
            var key = (ConsoleKey.DownArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(parameters);

            // assert
            Assert.That(parameters.Text.ToString(), Is.EqualTo(expectedInputHistory));
        }
    }
}