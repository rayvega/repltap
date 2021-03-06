using System.Threading.Tasks;
using System;
using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.Core;
using ReplTap.Core.History;
using static System.Environment;

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

            var inputHistory = new Mock<IInputHistory>();

            keyHandler
                .Setup(c => c.Process(consoleState.Object))
                .Returns(input);

            replEngine
                .Setup(r => r.Execute($"{input}{NewLine}"))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            consoleState
                .Setup(c => c.InputHistory)
                .Returns(inputHistory.Object);

            var consoleStateText = new StringBuilder();
            consoleStateText.Append("test text that should be cleared");

            Assert.That(consoleStateText.ToString(), Is.Not.Empty);

            consoleState
                .Setup(c => c.Text)
                .Returns(consoleStateText);

            var interactiveLoop = new InteractiveLoop(console.Object,
                keyHandler.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(output), Times.Once);
            consoleWriter.Verify(c => c.WriteError(output), Times.Never);

            inputHistory.Verify(i => i.Add(It.IsAny<string>()));

            consoleState.VerifySet(c => c.ColPosition = expectedColPosition);
            consoleState.VerifySet(c => c.RowPosition = expectedRowPosition);
            consoleState.VerifySet(c => c.TextRowPosition = 0, "should reset to zero");
            consoleState.VerifySet(c => c.TextRowPosition = 1, Times.Never, "should not increment");
            Assert.That(consoleStateText.ToString(), Is.Empty);
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
            var keyHandler = new Mock<IConsoleKeyHandler>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var consoleState = new Mock<IConsoleState>();

            var inputHistory = new Mock<IInputHistory>();

            keyHandler
                .Setup(c => c.Process(consoleState.Object))
                .Returns(input);

            replEngine
                .Setup(r => r.Execute($"{input}{NewLine}"))
                .ReturnsAsync(result);

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            consoleState
                .Setup(c => c.InputHistory)
                .Returns(inputHistory.Object);

            var interactiveLoop = new InteractiveLoop(console.Object,
                keyHandler.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            consoleWriter.Verify(c => c.WriteOutput(output), Times.Never);
            consoleWriter.Verify(c => c.WriteError(output), Times.Once);

            inputHistory.Verify(i => i.Add(It.IsAny<string>()));

            consoleState.VerifySet(c => c.TextRowPosition = 0, "should reset to zero");
            consoleState.VerifySet(c => c.TextRowPosition = 1, Times.Never, "should not increment");
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

            consoleState
                .Setup(c => c.InputHistory)
                .Returns(new Mock<IInputHistory>().Object);

            consoleState
                .SetupSequence(c => c.Prompt)
                .Returns(Prompt.Standard)
                .Returns(Prompt.Continue);

            var interactiveLoop = new InteractiveLoop(console.Object,
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, consoleState.Object);

            // act
            await interactiveLoop.Run();

            // assert
            console.Verify(c => c.Write($"{Prompt.Standard} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(firstResult.Output), Times.Never());

            console.Verify(c => c.Write($"{Prompt.Continue} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(secondResult.Output), Times.Once());

            consoleState.VerifySet(c => c.TextRowPosition = 1, Times.Once, "should increment");
            consoleState.VerifySet(c => c.TextRowPosition = 0, Times.Once, "should reset to zero");
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
                .Setup(r => r.Execute($"{input}{NewLine}"))
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
