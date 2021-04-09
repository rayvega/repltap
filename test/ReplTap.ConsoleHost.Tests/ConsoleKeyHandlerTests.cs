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
                .Returns(new Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>>());

            var calls = 0;

            consoleKeyCommands
                .Setup(c => c.WriteChar(It.IsAny<ConsoleState>(), It.IsAny<char>()))
                .Callback<ConsoleState, char>((state, _) =>
                {
                    var (inputChar, _) = consoleKeys[calls];
                    state.Text.Append(inputChar);
                    calls++;

                    Assert.That(state.ColPosition, Is.EqualTo(expectedCursorLeft));
                });

            // key handler
            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            var inputHistory = new Mock<IInputHistory>();

            // act
            var input = keyHandler.Process(Prompt.Standard, inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("abc"));

            consoleKeyCommands
                .Verify(c => c.WriteChar(It.IsAny<ConsoleState>(), It.IsAny<char>()), Times.Exactly(3));
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

            var map = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>>
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
                .Setup(c => c.WriteChar(It.IsAny<ConsoleState>(), It.IsAny<char>()))
                .Callback<ConsoleState, char>((state, _) =>
                {
                    var (inputChar, _) = consoleKeys[calls];
                    state.Text.Append(inputChar);
                    calls++;

                    Assert.That(state.ColPosition, Is.EqualTo(expectedCursorLeft));
                });

            // key handler
            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            var inputHistory = new Mock<IInputHistory>();

            // act
            var input = keyHandler.Process(Prompt.Standard, inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("abd"));
            Assert.IsTrue(isCommandCalled);
        }
    }
}
