using System.Threading.Tasks;
using System;
using Moq;
using NUnit.Framework;
using ReplTap.Core;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class InteractiveLoopTests
    {
        [Test]
        public async Task Run_Should_Output_Code_Results_When_Valid()
        {
            // arrange
            var input = "test code";
            var output = "test output";
            var result = new CodeResult
            {
                Output = output,
                State = OutputState.Valid,
            };

            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();

            consoleReader
                .Setup(c => c.ReadLine(It.IsAny<string>()))
                .ReturnsAsync(input);

            replEngine
                .Setup(r => r.Execute(input))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(output), Times.Once);
            consoleWriter.Verify(c => c.WriteError(output), Times.Never);
        }

        [Test]
        public async Task Run_Should_Output_Error_Message_From_Result_When_Error_State()
        {
            // arrange
            var input = "test invalid code";
            var output = "error output";
            var result = new CodeResult
            {
                Output = output,
                State = OutputState.Error,
            };

            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();

            consoleReader
                .Setup(c => c.ReadLine(It.IsAny<string>()))
                .ReturnsAsync(input);

            replEngine
                .Setup(r => r.Execute(input))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(output), Times.Never);
            consoleWriter.Verify(c => c.WriteError(output), Times.Once);
        }

        [Test]
        public async Task Run_Should_Output_Code_Results_When_Multiline_Continue()
        {
            // arrange
            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();

            consoleReader
                .SetupSequence(c => c.ReadLine(It.IsAny<string>()))
                .Returns(Task.FromResult("var "))
                .Returns(Task.FromResult("testVariable = \"test value\";"));

            var firstResult = new CodeResult
            {
                State = OutputState.Continue,
                Output = "no output after 1st input",
            };

            var secondResult = new CodeResult
            {
                State = OutputState.Valid,
                Output = "final output after 2nd input",
            };

            replEngine
                .SetupSequence(r => r.Execute(It.IsAny<string>()))
                .Returns(Task.FromResult(firstResult))
                .Returns(Task.FromResult(secondResult));

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);

            // act
            await interactiveLoop.Run();

            // assert
            console.Verify(c => c.Write($"{Prompt.Standard} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(firstResult.Output), Times.Never());

            console.Verify(c => c.Write($"{Prompt.Continue} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(secondResult.Output), Times.Once());
        }

        [Test]
        public async Task Run_Should_Output_Error_When_Throws_Exception()
        {
            // arrange
            var input = "test code";
            var errorOutput = "test error output";

            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();

            consoleReader
                .Setup(c => c.ReadLine(It.IsAny<string>()))
                .ReturnsAsync(input);

            var expectedException = new Exception(errorOutput);

            replEngine
                .Setup(r => r.Execute(input))
                .Throws(expectedException);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);

            // act && assert
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(errorOutput), Times.Never);
            consoleWriter.Verify(c => c.WriteError(expectedException), Times.Once);
        }
    }
}