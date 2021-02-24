using System.Text;
using Moq;
using NUnit.Framework;
using ReplTap.ConsoleHost.Commands;

namespace ReplTap.ConsoleHost.Tests.Commands
{
    public class EditCommandsTests
    {

        [Test]
        public void WriteChar_Should_Execute_Expected()
        {
            // arrange
            var console = new Mock<IConsole>();

            const int position = 4; // including prompt length of 2

            var state = new ConsoleState
            {
                Text = new StringBuilder().Append("line1\nabcde"),
                LinePosition = position,
            };

            var inputChar = 'z';
            var editCommands = new EditCommands(console.Object);

            // act
            editCommands.WriteChar(state, inputChar);

            // assert
            console.Verify(c => c.Write("zcde"));
            Assert.That(state.Text.ToString(), Is.EqualTo("line1\nabzcde"));
            Assert.That(state.LinePosition, Is.EqualTo(position + 1));
            console.VerifySet(c => c.CursorLeft = position + 1);
        }

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
            var editCommands = new EditCommands(console.Object);

            // act
            editCommands.Backspace(state);

            // assert
            console.Verify(c => c.Write(" "));
            Assert.That(state.Text.ToString(), Is.EqualTo("line1\ntest cod"));
            Assert.That(state.LinePosition, Is.EqualTo(position - 1));
            console.VerifySet(c => c.CursorLeft = position - 1);
        }
    }
}
