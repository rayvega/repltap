using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
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
            var console = new Mock<IConsole>();

            Console.CursorLeft = 2; // TODO: mock this

            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('a', It.IsAny<ConsoleKey>()),
                ('b', It.IsAny<ConsoleKey>()),
                ('c', ConsoleKey.Enter),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var inputHistory = new Mock<IInputHistory>();

            var consoleKeyCommands = new Mock<IConsoleKeyCommands>();

            consoleKeyCommands
                .Setup(c => c.GetInputKeyCommandMap())
                .Returns(new Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>>());

            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            // act
            var input = keyHandler.Process(It.IsAny<string>(), inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("abc"));
        }

        [Test]
        public void Process_Should_Return_Input_When_Key_Any_Other_Command()
        {
            // arrange
            var otherConsoleKey = ConsoleKey.F24;

            Console.CursorLeft = 2; // TODO: mock this

            var console = new Mock<IConsole>();
            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('a', It.IsAny<ConsoleKey>()),
                ('b', It.IsAny<ConsoleKey>()),
                ('c', otherConsoleKey),
                ('d', ConsoleKey.Enter),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var inputHistory = new Mock<IInputHistory>();

            var consoleKeyCommands = new Mock<IConsoleKeyCommands>();
            var isCommandCalled = false;

            var map = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>>
            {
                {(otherConsoleKey, (ConsoleModifiers) 0), parameters => { isCommandCalled = true; }}
            };

            consoleKeyCommands
                .Setup(c => c.GetInputKeyCommandMap())
                .Returns(map);

            var keyHandler = new ConsoleKeyHandler(console.Object, consoleKeyCommands.Object);

            // act
            var input = keyHandler.Process(It.IsAny<string>(), inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("abd"));
            Assert.IsTrue(isCommandCalled);
        }
    }
}
