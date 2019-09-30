using System;
using System.Threading.Tasks;
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
                Output = output
            };

            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var inputCheck = new Mock<IInputCheck>();

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
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, inputCheck.Object);
            
            // act
            await interactiveLoop.Run();
            
            // assert
            consoleWriter.Verify(c => c.WriteOutput(output), Times.Once);
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
            var inputCheck = new Mock<IInputCheck>();

            consoleReader
                .Setup(c => c.ReadLine(It.IsAny<string>()))
                .ReturnsAsync(input);
            
            replEngine
                .Setup(r => r.Execute(input))
                .Throws(new Exception(errorOutput));

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object, 
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, inputCheck.Object);
            
            // act && assert
            await interactiveLoop.Run();
            
            // assert
            consoleWriter.Verify(c => c.WriteError(errorOutput), Times.Once);
        }

        [Test]
        public async Task Run_Should_Output_Code_Results_When_Multiline()
        {
            // arrange
            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var inputCheck = new Mock<IInputCheck>();

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

            inputCheck
                .Setup(m => m.IsForceExecute(It.IsAny<string>()))
                .Returns(false);

            var interactiveLoop = new InteractiveLoop(console.Object, 
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, inputCheck.Object);
            
            // act
            await interactiveLoop.Run();
            
            // assert
            console.Verify(c => c.Write($"{Prompt.Standard} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(firstResult.Output), Times.Never());

            console.Verify(c => c.Write($"{Prompt.Continue} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(secondResult.Output), Times.Once());
        }

        [Test]
        public async Task Run_Should_Output_Code_Results_When_Input_Continue_But_Force_Execute()
        {
            // arrange
            var console = new Mock<IConsole>();
            var consoleReader = new Mock<IConsoleReader>();
            var consoleWriter = new Mock<IConsoleWriter>();
            var replEngine = new Mock<IReplEngine>();
            var loop = new Mock<ILoop>();
            var inputCheck = new Mock<IInputCheck>();

            consoleReader
                .SetupSequence(c => c.ReadLine(It.IsAny<string>()))
                .Returns(Task.FromResult("input with force execute"));

            var firstResult = new CodeResult
            {
                State = OutputState.Continue,
                Output = "output for input force execute",
            };

            replEngine
                .SetupSequence(r => r.Execute(It.IsAny<string>()))
                .Returns(Task.FromResult(firstResult));

            loop
                .SetupSequence(l => l.Continue())
                .Returns(true)
                .Returns(false);

            inputCheck
                .Setup(m => m.IsForceExecute(It.IsAny<string>()))
                .Returns(true);

            var interactiveLoop = new InteractiveLoop(console.Object, 
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object, inputCheck.Object);
            
            // act
            await interactiveLoop.Run();
            
            // assert
            console.Verify(c => c.Write($"{Prompt.Standard} "), Times.Once());
            consoleWriter.Verify(c => c.WriteOutput(firstResult.Output), Times.Once());

            console.Verify(c => c.Write($"{Prompt.Continue} "), Times.Never());
        }
    }
}