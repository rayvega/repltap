using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class ConsoleReaderTests
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

            var consoleReader = new ConsoleReader(console.Object, provider.Object);

            // act
            var input = await consoleReader.ReadLine(prompt);

            // assert
            Assert.That(input, Is.EqualTo("abc"));

            provider.Verify(p => p.GetCompletions(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ReadLine_Should_Return_Write_Completions_When_Key_Tab()
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

            var provider = new Mock<ICompletionsProvider>();
            var consoleReader = new ConsoleReader(console.Object, provider.Object);

            // act
            var input = await consoleReader.ReadLine(prompt);

            // assert
            Assert.That(input, Is.EqualTo("abd"));

            provider.Verify(p => p.GetCompletions("ab"), Times.Once());
            console.Verify(c => c.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());
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

            var provider = new Mock<ICompletionsProvider>();

            var consoleReader = new ConsoleReader(console.Object, provider.Object);

            // act
            var input = await consoleReader.ReadLine(prompt);

            // assert
            Assert.That(input, Is.EqualTo("efh"));

            provider.Verify(p => p.GetCompletions(It.IsAny<string>()), Times.Never);
        }
    }
}