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

            // add two pre-existing lines
            state.Text.Append("123\n456\n");

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
            Assert.That(input, Is.EqualTo("123\n456\nabc"), "should have added a third line of code");

            consoleKeyCommands
                .Verify(c => c.WriteChar(state, It.IsAny<char>()), Times.Exactly(3));

            console
                .VerifySet(c =>
                {
                    var expectedTotalNumberOfLines = 3;

                    c.CursorTop = expectedTotalNumberOfLines;
                });
        }

        [Test]
        public void Process_Should_Return_Input_When_Key_Any_Other_Command()
        {
            // arrange
            var otherConsoleKey = ConsoleKey.F24;

            var console = new Mock<IConsole>();

            // read key
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

            console
                .VerifySet(c =>
                {
                    var expectedTotalNumberOfLines = 1;

                    c.CursorTop = expectedTotalNumberOfLines;
                });
        }
    }
}
