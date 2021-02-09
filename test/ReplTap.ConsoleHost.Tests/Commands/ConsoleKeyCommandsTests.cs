using System;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    [TestFixture]
    public class ConsoleKeyCommandsTests
    {
        [Test]
        public void WriteChar_Should_Execute_Expected()
        {
            // arrange
            var console = new Mock<IConsole>();
            var keyCommands = new ConsoleKeyCommands(console.Object, null!);

            const int position = 4; // including prompt length of 2

            var state = new ConsoleState
            {
                Text = new StringBuilder().Append("line1\nabcde"),
                LinePosition = position,
            };

            var inputChar = 'z';

            // act
            keyCommands.WriteChar(state, inputChar);

            // assert
            console.Verify(c => c.Write("zcde"));
            Assert.That(state.Text.ToString(), Is.EqualTo("line1\nabzcde"));
            Assert.That(state.LinePosition, Is.EqualTo(position + 1));
            console.VerifySet(c => c.CursorLeft = position + 1);
        }

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

            var state = new ConsoleState
            {
                Text = text,
                Variables = variables,
                InputHistory = inputHistory.Object,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, completionsWriter.Object);
            var key = (ConsoleKey.Tab, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            completionsWriter.Verify(
                p => p.WriteAllCompletions($"all test history inputs{Environment.NewLine}test code", variables),
                Times.Once());
        }

        [Test]
        public void Map_Command_Should_Return_Smaller_Input_When_Key_Backspace()
        {
            // arrange
            var lineText = "test code";
            var text = new StringBuilder();
            text.Append($"line1\n{lineText}");

            var position = Prompt.Standard.Length + lineText.Length + 1;

            var state = new ConsoleState
            {
                Text = text,
                LinePosition = position,
            };

            var completionsWriter = new Mock<ICompletionsWriter>();
            var console = new Mock<IConsole>();
            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, completionsWriter.Object);

            var key = (ConsoleKey.Backspace, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            console.Verify(c => c.Write(" "));
            Assert.That(state.Text.ToString(), Is.EqualTo("line1\ntest cod"));
            Assert.That(state.LinePosition, Is.EqualTo(position - 1));
            console.VerifySet(c => c.CursorLeft = position - 1);
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Up_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history\rline 2\rline3";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetPreviousInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                InputHistory = inputHistory.Object,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.UpArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.Text.ToString(), Is.EqualTo(expectedInputHistory));
            console.Verify(c => c.Write(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Down_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history\rline 2\rline3";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetNextInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                InputHistory = inputHistory.Object,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.DownArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.Text.ToString(), Is.EqualTo(expectedInputHistory));
            console.Verify(c => c.Write(It.IsAny<string>()), Times.Exactly(3));
        }


        [Test]
        public void Map_Command_Should_Move_Left_When_Key_Left_Arrow()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                LinePosition = 4,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.LeftArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.LinePosition, Is.EqualTo(3));
        }

        [Test]
        public void Map_Command_Should_Not_Move_Left_When_Key_Left_Arrow_And_Start_Of_Line()
        {
            // arrange
            var originalLinePosition = Prompt.Standard.Length + 1;

            var state = new ConsoleState
            {
                LinePosition = originalLinePosition,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.LeftArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.LinePosition, Is.EqualTo(originalLinePosition));
        }

        [Test]
        public void Map_Command_Should_Move_Right_When_Key_Right_Arrow()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                LinePosition = 4,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.RightArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.LinePosition, Is.EqualTo(5));
        }

        [Test]
        public void Map_Command_Should_Move_Not_Right_When_Key_Right_Arrow_And_End_Of_Line()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var originalLinePosition = Prompt.Standard.Length + text.Length + 1;

            var state = new ConsoleState
            {
                Text = text,
                LinePosition = originalLinePosition,
            };

            var console = new Mock<IConsole>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, null!);
            var key = (ConsoleKey.RightArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            Assert.That(state.LinePosition, Is.EqualTo(originalLinePosition));
        }
    }
}
