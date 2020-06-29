using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ReplTap.Core.Completions;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class ConsoleKeyHandlerTests
    {
        [Test]
        public async Task ReadLine_Should_Return_Input_When_Key_Enter()
        {
            // arrange
            var prompt = "test prompt *";
            var console = new Mock<IConsole>();
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

            var provider = new Mock<ICompletionsProvider>();
            var keyHandler = new ConsoleKeyHandler(console.Object, null!);
            var inputHistory = new Mock<IInputHistory>();

            // act
            var input = await keyHandler.ReadLine(prompt, inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("abc"));

            provider.Verify(p => p.GetCompletions(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ReadLine_Should_Write_All_Completions_When_Key_Tab()
        {
            // arrange
            var prompt = "test prompt *";

            var console = new Mock<IConsole>();
            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('a', It.IsAny<ConsoleKey>()),
                ('b', It.IsAny<ConsoleKey>()),
                (It.IsAny<char>(), ConsoleKey.Tab),
                ('d', ConsoleKey.Enter),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var completionsWriter = new Mock<ICompletionsWriter>();
            var keyHandler = new ConsoleKeyHandler(console.Object, completionsWriter.Object);

            var inputHistory = new Mock<IInputHistory>();
            inputHistory
                .Setup(i => i.AllInputsAsString())
                .Returns("all test history inputs");

            var variables = Enumerable
                .Range(1, 3)
                .Select(i => $"test variable {i}")
                .ToList();

            // act
            var input = await keyHandler.ReadLine(prompt, inputHistory.Object, variables);

            // assert
            Assert.That(input, Is.EqualTo("abd"));

            completionsWriter.Verify(
                p => p.WriteAllCompletions($"all test history inputs{Environment.NewLine}ab", variables),
                Times.Once());
        }

        [Test]
        public async Task ReadLine_Should_Return_Smaller_Input_When_Key_Backspace()
        {
            // arrange
            var prompt = "test prompt *";

            var console = new Mock<IConsole>();
            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey)>
            {
                ('e', It.IsAny<ConsoleKey>()),
                ('f', It.IsAny<ConsoleKey>()),
                ('g', It.IsAny<ConsoleKey>()),
                (It.IsAny<char>(), ConsoleKey.Backspace),
                ('h', ConsoleKey.Enter),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, false, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var completionsWriter = new Mock<ICompletionsWriter>();
            var keyHandler = new ConsoleKeyHandler(console.Object, null!);
            var inputHistory = new Mock<IInputHistory>();

            // act
            var input = await keyHandler.ReadLine(prompt, inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo("efh"));

            completionsWriter.Verify(p => p.WriteAllCompletions(It.IsAny<string>(), It.IsAny<List<string>>()),
                Times.Never);
        }

        [Test]
        public async Task ReadLine_Should_Return_Input_History_When_Key_Up_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history";
            var lineEndingToBeRemoved = "\n";

            var console = new Mock<IConsole>();
            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey, bool altKey)>
            {
                ('a', It.IsAny<ConsoleKey>(), false),
                ('b', It.IsAny<ConsoleKey>(), false),
                ('c', It.IsAny<ConsoleKey>(), false),
                (It.IsAny<char>(), ConsoleKey.UpArrow, true),
                (' ', ConsoleKey.Enter, false),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey, altKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, altKey, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var completionsWriter = new Mock<ICompletionsWriter>();

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetPreviousInput())
                .Returns($"{expectedInputHistory}{lineEndingToBeRemoved}");

            var keyHandler = new ConsoleKeyHandler(console.Object, null!);

            // act
            var input = await keyHandler.ReadLine(It.IsAny<string>(), inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo(expectedInputHistory));

            completionsWriter.Verify(p => p.WriteAllCompletions(It.IsAny<string>(), It.IsAny<List<string>>()),
                Times.Never);
        }

        [Test]
        public async Task ReadLine_Should_Return_Input_History_When_Key_Down_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history";
            var lineEndingToBeRemoved = "\n";

            var console = new Mock<IConsole>();
            var consoleKeys = new List<(char inputChar, ConsoleKey consoleKey, bool altKey)>
            {
                ('a', It.IsAny<ConsoleKey>(), false),
                ('b', It.IsAny<ConsoleKey>(), false),
                ('c', It.IsAny<ConsoleKey>(), false),
                (It.IsAny<char>(), ConsoleKey.DownArrow, true),
                (' ', ConsoleKey.Enter, false),
            };

            var setupSequence = console
                .SetupSequence(c => c.ReadKey(true));

            foreach (var (inputChar, consoleKey, altKey) in consoleKeys)
            {
                var consoleKeyInfo = new ConsoleKeyInfo(inputChar, consoleKey, false, altKey, false);

                setupSequence.Returns(consoleKeyInfo);
            }

            var completionsWriter = new Mock<ICompletionsWriter>();

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetNextInput())
                .Returns($"{expectedInputHistory}{lineEndingToBeRemoved}");

            var keyHandler = new ConsoleKeyHandler(console.Object, null!);

            // act
            var input = await keyHandler.ReadLine(It.IsAny<string>(), inputHistory.Object, null!);

            // assert
            Assert.That(input, Is.EqualTo(expectedInputHistory));

            completionsWriter.Verify(p => p.WriteAllCompletions(It.IsAny<string>(), It.IsAny<List<string>>()),
                Times.Never);
        }
    }
}