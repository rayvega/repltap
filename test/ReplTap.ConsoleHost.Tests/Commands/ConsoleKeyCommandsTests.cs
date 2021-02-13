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
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var keyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object, editCommands.Object,
                null!, inputHistoryCommands.Object);
            var state = new ConsoleState();
            var inputChar = 'z';

            // act
            keyCommands.WriteChar(state, inputChar);

            // assert
            editCommands.Verify(e => e.WriteChar(state, inputChar), Times.Once);
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
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, completionsWriter.Object, inputHistoryCommands.Object);
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
            var state = new ConsoleState();

            var console = new Mock<IConsole>();
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var completionsWriter = new Mock<ICompletionsWriter>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, completionsWriter.Object, inputHistoryCommands.Object);

            var key = (ConsoleKey.Backspace, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            editCommands.Verify(e => e.Backspace(state), Times.Once);
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Up_Arrow()
        {
            // arrange
            var state = new ConsoleState();

            var console = new Mock<IConsole>();
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.UpArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            inputHistoryCommands.Verify(i => i.PreviousInput(state), Times.Once);
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Down_Arrow()
        {
            // arrange
            var state = new ConsoleState();

            var console = new Mock<IConsole>();
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.DownArrow, ConsoleModifiers.Alt);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            inputHistoryCommands.Verify(i => i.NextInput(state), Times.Once);
        }


        [Test]
        public void Map_Command_Should_Move_Left_When_Key_Left_Arrow()
        {
            // arrange
            var state = new ConsoleState();

            var console = new Mock<IConsole>();
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.LeftArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            navigateCommands.Verify(n => n.MoveCursorLeft(state), Times.Once);
        }

        [Test]
        public void Map_Command_Should_Move_Right_When_Key_Right_Arrow()
        {
            // arrange
            var state = new ConsoleState();

            var console = new Mock<IConsole>();
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(console.Object, navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.RightArrow, (ConsoleModifiers) 0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            navigateCommands.Verify(n => n.MoveCursorRight(state), Times.Once);
        }
    }
}
