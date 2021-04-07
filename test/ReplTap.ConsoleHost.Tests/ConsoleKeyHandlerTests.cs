using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class ConsoleKeyHandlerTests
    {
        [Test]
        public void Process_Should_Return_Input_When_Key_Enter()
        {
            // arrange

            // read key
            var console = new Mock<IConsole>();

            var expectedCursorLeft = $"{Prompt.Standard} ".Length;

            console
                .Setup(c => c.CursorLeft)
                .Returns(expectedCursorLeft);

            var expectedCursorTop = 5;

            console
                .Setup(c => c.CursorTop)
                .Returns(expectedCursorTop);

            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('a', It.IsAny<ConsoleKey>()),
                ('b', It.IsAny<ConsoleKey>()),
                ('c', It.IsAny<ConsoleKey>()),
                (' ', ConsoleKey.Enter),
            };

            var readKeySetupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                readKeySetupSequence.Returns(consoleKeyInfo);
            }

            // key commands
            var consoleKeyCommands = new Mock<IConsoleKeyCommands>();

            consoleKeyCommands
                .Setup(c => c.GetInputKeyCommandMap())
                .Returns(new Dictionary<(ConsoleKey, ConsoleModifiers), Action<IConsoleState>>());

            var inputHistory = new Mock<IInputHistory>();

            var state = new ConsoleState(inputHistory.Object);

            var calls = 0;

            consoleKeyCommands
                .Setup(c => c.WriteChar(state, It.IsAny<char>()))
                .Callback<IConsoleState, char>((s, _) =>
                {
                    var (inputChar, _) = consoleKeys[calls];
                    s.Text.Append(inputChar);
                    calls++;
                });

            // key handler
            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);


            // act
            var input = keyHandler.Process(state);

            // assert
            Assert.That(input, Is.EqualTo("abc"));

            consoleKeyCommands
                .Verify(c => c.WriteChar(state, It.IsAny<char>()), Times.Exactly(3));
        }

        [Test]
        public void Process_Should_Return_Input_When_Key_Any_Other_Command()
        {
            // arrange
            var otherConsoleKey = ConsoleKey.F24;

            var console = new Mock<IConsole>();

            // read key

            var expectedCursorLeft = $"{Prompt.Standard} ".Length;

            console
                .Setup(c => c.CursorLeft)
                .Returns(expectedCursorLeft);

            var expectedCursorTop = 5;

            console
                .Setup(c => c.CursorTop)
                .Returns(expectedCursorTop);

            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('a', It.IsAny<ConsoleKey>()),
                ('b', It.IsAny<ConsoleKey>()),
                ('c', otherConsoleKey),
                ('d', It.IsAny<ConsoleKey>()),
                (' ', ConsoleKey.Enter),
            };

            var readKeySetupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                readKeySetupSequence.Returns(consoleKeyInfo);
            }

            // key commands
            var calls = 0;

            var isCommandCalled = false;

            var map = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<IConsoleState>>
            {
                {(otherConsoleKey, 0), _ =>
                    {
                        isCommandCalled = true;
                        calls++;
                    }
                }
            };

            var consoleKeyCommands = new Mock<IConsoleKeyCommands>();

            consoleKeyCommands
                .Setup(c => c.GetInputKeyCommandMap())
                .Returns(map);

            consoleKeyCommands
                .Setup(c => c.WriteChar(It.IsAny<IConsoleState>(), It.IsAny<char>()))
                .Callback<IConsoleState, char>((s, _) =>
                {
                    var (inputChar, _) = consoleKeys[calls];
                    s.Text.Append(inputChar);
                    calls++;
                });

            // key handler
            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            var inputHistory = new Mock<IInputHistory>();
            var state = new ConsoleState(inputHistory.Object);

            // act
            var input = keyHandler.Process(state);

            // assert
            Assert.That(input, Is.EqualTo("abd"));
            Assert.IsTrue(isCommandCalled);
        }

        [Test]
        public void Process_Should_Initially_Clear_Text_From_Console_State()
        {
            // arrange
            var console = new Mock<IConsole>();
            var consoleKeyCommands = new Mock<IConsoleKeyCommands>();
            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            var consoleKeyInfo = new ConsoleKeyInfo(' ', ConsoleKey.Enter, false, false, false);
            console
                .Setup(c => c.ReadKey(true))
                .Returns(consoleKeyInfo);

            var inputHistory = new Mock<IInputHistory>();
            var state = new ConsoleState(inputHistory.Object);

            state.Text.Append("test text that should be removed");

            // act
            keyHandler.Process(state);

            // assert
            Assert.That(state.Text.ToString(), Is.Empty);
        }
    }
}
