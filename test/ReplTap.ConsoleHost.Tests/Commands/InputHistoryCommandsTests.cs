using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    [TestFixture]
    public class InputHistoryCommandsTests
    {
        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Up_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history\rline 2\rline3";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetPreviousInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState(new InputHistory())
            {
                Text = text,
                InputHistory = inputHistory.Object,
            };

            var console = new Mock<IConsole>();
            var inputHistoryCommands = new InputHistoryCommands(console.Object);

            // act
            inputHistoryCommands.PreviousInput(state);

            // assert
            Assert.That(state.Text.ToString(), Is.EqualTo(expectedInputHistory));
            console.Verify(c => c.Write(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public void Map_Command_Should_Return_Input_History_When_Key_Down_Arrow()
        {
            // arrange
            var expectedInputHistory = "test input from history\rline 2\rline3";

            var inputHistory = new Mock<IInputHistory>();

            inputHistory
                .Setup(i => i.GetNextInput())
                .Returns(expectedInputHistory);

            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState(new InputHistory())
            {
                Text = text,
                InputHistory = inputHistory.Object,
            };

            var console = new Mock<IConsole>();

            var inputHistoryCommands = new InputHistoryCommands(console.Object);

            // act
            inputHistoryCommands.NextInput(state);

            // assert
            Assert.That(state.Text.ToString(), Is.EqualTo(expectedInputHistory));
            console.Verify(c => c.Write(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
