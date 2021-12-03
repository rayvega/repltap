using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
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
                Variables = Enumerable.Range(1, 2).Select(i => $"test-var-{i}").ToList(),
            };

            var console = new Mock<IConsole>();

            var expectedColPosition = 3;
            console
                .Setup(c => c.CursorLeft)
                .Returns(expectedColPosition);

            var expectedRowPosition = 6;
            console
                .Setup(c => c.CursorTop)
                .Returns(expectedRowPosition);

            var keyHandler = new Mock<IConsoleKeyHandler>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var consoleState = new Mock<IConsoleState>();

            keyHandler
                .Setup(c => c.Process(consoleState.Object))
                .Returns(input);

            replEngine
                .Setup(r => r.Execute(input))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                keyHandler.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleState.VerifySet(c => c.ColPosition = expectedColPosition);
            consoleState.VerifySet(c => c.RowPosition = expectedRowPosition);

            consoleWriter.Verify(c => c.WriteOutput(output), Times.Once);
            consoleWriter.Verify(c => c.WriteError(output), Times.Never);

            consoleState.Verify(c => c.CompleteInput(result.Variables));
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
                Variables = Enumerable.Range(1, 3).Select(i => $"test-var-{i}").ToList(),
            };

            var console = new Mock<IConsole>();
            var keyHandler = new Mock<IConsoleKeyHandler>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var consoleState = new Mock<IConsoleState>();

            keyHandler
                .Setup(c => c.Process(consoleState.Object))
                .Returns(input);

            replEngine
                .Setup(r => r.Execute(input))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            consoleState
                .Setup(c => c.Text)
                .Returns(new StringBuilder());

            var interactiveLoop = new InteractiveLoop(console.Object,
                keyHandler.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleState.VerifySet(c => c.TextRowPosition = 1, Times.Never, "should not increment");

            consoleWriter.Verify(c => c.WriteOutput(output), Times.Never);
            consoleWriter.Verify(c => c.WriteError(output), Times.Once);

            consoleState.Verify(c => c.CompleteInput(result.Variables));
        }

        [Test]
        public async Task Run_Should_Output_Code_Results_When_Multiline_Continue()
        {
            // arrange
            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleKeyHandler>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var consoleState = new Mock<IConsoleState>();

            consoleReader
                .SetupSequence(c => c.Process(consoleState.Object))
                .Returns("var ")
                .Returns("testVariable = \"test value\";");

            var firstResult = new CodeResult
            {
                State = OutputState.Continue,
                Output = "no output after 1st input",
                Variables = Enumerable.Range(1, 1).Select(i => $"test-var-{i}").ToList(),
            };

            var secondResult = new CodeResult
            {
                State = OutputState.Valid,
                Output = "final output after 2nd input",
                Variables = Enumerable.Range(1, firstResult.Variables.Count + 1).Select(i => $"test-var-{i}").ToList(),
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

            consoleState
                .SetupSequence(c => c.Prompt)
                .Returns(Prompt.Standard)
                .Returns(Prompt.Continue);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            console.Verify(c => c.Write($"{Prompt.Standard} "), Times.Once);
            consoleWriter.Verify(c => c.WriteOutput(firstResult.Output), Times.Never);

            console.Verify(c => c.Write($"{Prompt.Continue} "), Times.Once);
            consoleWriter.Verify(c => c.WriteOutput(secondResult.Output), Times.Once);

            consoleState.VerifySet(c => c.TextRowPosition = 1, Times.Once, "should increment");

            consoleState.Verify(c => c.CompleteInput(secondResult.Variables), Times.Once);
        }

        [Test]
        public async Task Run_Should_Output_Error_When_Throws_Exception()
        {
            // arrange
            var input = "test code";
            var errorOutput = "test error output";

            var console = new Mock<IConsole>();
            var keyHandler = new Mock<IConsoleKeyHandler>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var consoleState = new Mock<IConsoleState>();

            keyHandler
                .Setup(c => c.Process(consoleState.Object))
                .Returns(input);

            var expectedException = new Exception(errorOutput);

            replEngine
                .Setup(r => r.Execute(input))
                .Throws(expectedException);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object,
                keyHandler.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act && assert
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(errorOutput), Times.Never);
            consoleWriter.Verify(c => c.WriteError(expectedException), Times.Once);
        }
    }
}
