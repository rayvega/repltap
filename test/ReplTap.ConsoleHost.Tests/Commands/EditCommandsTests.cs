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

            const int position = 1;

            var builder = new StringBuilder("line1\nline2");

            var state = new Mock<IConsoleState>();

            state
                .Setup(s => s.CurrentLineText)
                .Returns("line2");

            state
                .Setup(s => s.TextColPosition)
                .Returns(position);

            state
                .Setup(s => s.Text)
                .Returns(builder);

            var colPosition = 5;

            state
                .Setup(s => s.ColPosition)
                .Returns(colPosition);

            var editCommands = new EditCommands(console.Object);

            // act
            editCommands.WriteChar(state.Object, 'z');

            // assert
            console.Verify(c => c.Write("zine2"));
            state.VerifySet(c => c.CurrentLineText = "lzine2");
            console.VerifySet(c => c.CursorLeft = colPosition + 1);
        }

        [Test]
        [TestCase("test code", 9, " ", "test cod")]
        [TestCase("test code", 8, "e ", "test coe")]
        [TestCase("test code", 7, "de ", "test cde")]
        public void Backspace_Execute_Expected(string lineText, int textPosition,
            string expectedWriteText, string expectedCurrentLineText)
        {
            // arrange
            var position = Prompt.Standard.Length + 1 + textPosition;

            var state = new Mock<IConsoleState>();

            state
                .SetupSequence(s => s.ColPosition)
                .Returns(position)
                .Returns(position - 1)
                .Returns(position - 1);

            state
                .Setup(s => s.CurrentLineText)
                .Returns(lineText);

            state
                .Setup(s => s.TextColPosition)
                .Returns(position - (Prompt.Standard.Length + 1) - 1);

            var console = new Mock<IConsole>();

            var editCommands = new EditCommands(console.Object);

            // act
            editCommands.Backspace(state.Object);

            // assert
            console.VerifySet(c => c.CursorLeft = position - 1);
            console.Verify(c => c.Write(expectedWriteText));
            state.VerifySet(c => c.CurrentLineText = expectedCurrentLineText);
            console.VerifySet(c => c.CursorLeft = position - 1);
        }
    }
}
