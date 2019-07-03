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
                .ReturnsAsync((output));

            var count = 0;
            
            loop
                .Setup(l => l.Continue())
                .Returns(() =>
                {
                    count++;

                    var shouldContinue = count <= 1;
                    
                    return shouldContinue;
                });
            
            var interactiveLoop = new InteractiveLoop(console.Object, 
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);
            
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

            consoleReader
                .Setup(c => c.ReadLine(It.IsAny<string>()))
                .ReturnsAsync(input);
            
            replEngine
                .Setup(r => r.Execute(input))
                .Throws(new Exception(errorOutput));

            var count = 0;
            
            loop
                .Setup(l => l.Continue())
                .Returns(() =>
                {
                    count++;

                    var shouldContinue = count <= 1;
                    
                    return shouldContinue;
                });
            
            var interactiveLoop = new InteractiveLoop(console.Object, 
                consoleReader.Object, consoleWriter.Object, replEngine.Object, loop.Object);
            
            // act && assert
            await interactiveLoop.Run();
            
            // assert
            consoleWriter.Verify(c => c.WriteError(errorOutput), Times.Once);
        }
    }
}