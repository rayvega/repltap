using System;
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
            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var keyCommands = new ConsoleKeyCommands(navigateCommands.Object, editCommands.Object,
                null!, inputHistoryCommands.Object);
            var state = new ConsoleState(new InputHistory());
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
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var completionsCommands = new Mock<ICompletionsCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
                editCommands.Object, completionsCommands.Object, null!);
            var key = (ConsoleKey.Tab, (ConsoleModifiers)0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            completionsCommands.Verify(e => e.Completions(state), Times.Once);
        }

        [Test]
        public void Map_Command_Should_Return_Smaller_Input_When_Key_Backspace()
        {
            // arrange
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
                editCommands.Object, null!, null!);

            var key = (ConsoleKey.Backspace, (ConsoleModifiers)0);

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
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
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
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
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
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.LeftArrow, (ConsoleModifiers)0);

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
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.RightArrow, (ConsoleModifiers)0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            navigateCommands.Verify(n => n.MoveCursorRight(state), Times.Once);
        }

        [Test]
        public void Map_Command_Should_Move_Up_When_Key_Up_Arrow()
        {
            // arrange
            var state = new ConsoleState(new InputHistory());

            var navigateCommands = new Mock<INavigateCommands>();
            var editCommands = new Mock<IEditCommands>();
            var inputHistoryCommands = new Mock<IInputHistoryCommands>();

            var consoleKeyCommands = new ConsoleKeyCommands(navigateCommands.Object,
                editCommands.Object, null!, inputHistoryCommands.Object);
            var key = (ConsoleKey.UpArrow, (ConsoleModifiers)0);

            // act
            var map = consoleKeyCommands.GetInputKeyCommandMap();
            var runCommand = map[key];
            runCommand(state);

            // assert
            navigateCommands.Verify(n => n.MoveCursorUp(state), Times.Once);
        }
    }
}
