using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    public class EditCommandsTests
    {
        [Test]
        public void Map_Command_Should_Return_Smaller_Input_When_Key_Backspace()
        {
            // arrange
            var lineText = "test code";
            var text = new StringBuilder();
            text.Append($"line1\n{lineText}");

            var position = Prompt.Standard.Length + lineText.Length + 1;

            var state = new ConsoleState
            {
                Text = text,
                LinePosition = position,
            };

            var console = new Mock<IConsole>();

            // act
            new EditCommands(console.Object).Backspace(state);

            // assert
            console.Verify(c => c.Write(" "));
            Assert.That(state.Text.ToString(), Is.EqualTo("line1\ntest cod"));
            Assert.That(state.LinePosition, Is.EqualTo(position - 1));
            console.VerifySet(c => c.CursorLeft = position - 1);
        }
    }
}
