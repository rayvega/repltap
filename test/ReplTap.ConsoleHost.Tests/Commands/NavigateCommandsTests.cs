using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    [TestFixture]
    public class NavigateCommandsTests
    {
        [Test]
        public void Map_Command_Should_Move_Left_When_Key_Left_Arrow()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                ColPosition = 4,
            };

            var console = new Mock<IConsole>();

            var navigateCommands = new NavigateCommands(console.Object);

            // act
            navigateCommands.MoveCursorLeft(state);

            // assert
            Assert.That(state.ColPosition, Is.EqualTo(3));
        }

        [Test]
        public void Map_Command_Should_Not_Move_Left_When_Key_Left_Arrow_And_Start_Of_Line()
        {
            // arrange
            var originalLinePosition = Prompt.Standard.Length + 1;

            var state = new ConsoleState
            {
                ColPosition = originalLinePosition,
            };

            var console = new Mock<IConsole>();

            var navigateCommands = new NavigateCommands(console.Object);

            // act
            navigateCommands.MoveCursorLeft(state);

            // assert
            Assert.That(state.ColPosition, Is.EqualTo(originalLinePosition));
        }

        [Test]
        public void Map_Command_Should_Move_Right_When_Key_Right_Arrow()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var state = new ConsoleState
            {
                Text = text,
                ColPosition = 4,
            };

            var console = new Mock<IConsole>();

            var navigateCommands = new NavigateCommands(console.Object);

            // act
            navigateCommands.MoveCursorRight(state);

            // assert
            Assert.That(state.ColPosition, Is.EqualTo(5));
        }

        [Test]
        public void Map_Command_Should_Not_Move_Right_When_Key_Right_Arrow_And_End_Of_Line()
        {
            // arrange
            var text = new StringBuilder();
            text.Append("test code");

            var originalLinePosition = Prompt.Standard.Length + text.Length + 1;

            var state = new ConsoleState
            {
                Text = text,
                ColPosition = originalLinePosition,
            };

            var console = new Mock<IConsole>();

            var navigateCommands = new NavigateCommands(console.Object);

            // act
            navigateCommands.MoveCursorRight(state);

            // assert
            Assert.That(state.ColPosition, Is.EqualTo(originalLinePosition));
        }
    }
}
