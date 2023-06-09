using System.Linq;
using Moq;
using NUnit.Framework;
using ReplTap.Core.History;
using static System.Environment;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class ConsoleStateTests
    {
        [Test]
        public void TextSplitLines_Should_Return_Expected()
        {
            // arrange
            var state = new ConsoleState(new InputHistory());
            state.Text.Append($"test code line 1{NewLine}test code line 2{NewLine}test code line 3");

            // act
            var lines = state.TextSplitLines;

            // assert
            Assert.That(lines.Length, Is.EqualTo(3));
        }

        [Test]
        [TestCase("line 1\nline 2\nline 3", 0, "line 1")]
        [TestCase("line 1\nline 2\nline 3", 1, "line 2")]
        [TestCase("line 1\nline 2\nline 3", 2, "line 3")]
        [TestCase("", 0, "")]
        [TestCase("a", 0, "a")]
        [TestCase("ab", 0, "ab")]
        [TestCase("a\n", 1, "")]
        [TestCase("a\nb", 1, "b")]
        public void CurrentLineText_Should_Return_Expected_Line_When_Row_Position(
            string fullText, int textRowPosition, string expectedText)
        {
            // arrange
            var state = new ConsoleState(new InputHistory());

            state.Text.Append(fullText);
            state.TextRowPosition = textRowPosition;

            // act
            var line = state.CurrentLineText;

            // assert
            Assert.That(line, Is.EqualTo(expectedText));
            Assert.That(state.Text.ToString(), Is.EqualTo(fullText));
        }

        [Test]
        [TestCase(0, "new test line\nline 2\nline3")]
        [TestCase(1, "line 1\nnew test line\nline3")]
        [TestCase(2, "line 1\nline 2\nnew test line")]
        public void CurrentLineText_Should_Set_Expected(int textRowPosition, string allText)
        {
            // arrange
            var state = new ConsoleState(new InputHistory());

            var initialText = "line 1\nline 2\nline3";
            state.Text.Append(initialText);

            var expectedCurrentLineText = "new test line";

            // act
            state.TextRowPosition = textRowPosition;
            state.CurrentLineText = expectedCurrentLineText;

            // assert
            var line = state.CurrentLineText;

            Assert.That(state.Text.ToString(), Is.EqualTo(allText));
            Assert.That(line, Is.EqualTo(expectedCurrentLineText));
        }

        [Test]
        [TestCase(8, true)]
        [TestCase(7, true)]
        [TestCase(6, false)]
        [TestCase(5, false)]
        [TestCase(4, false)]
        [TestCase(3, false)]
        [TestCase(2, false)]
        [TestCase(1, false)]
        [TestCase(0, false)]
        [TestCase(-1, false)]
        public void IsEndOfTextPosition_Should_Return_Expected(int linePosition, bool expectedIsEnd)
        {
            // arrange
            var state = new ConsoleState(new InputHistory())
            {
                ColPosition = linePosition
            };

            state.Text.Append("line1\nline2\nline3");

            // act
            var isEndOfTextPosition = state.IsEndOfTextPosition();

            // assert
            Assert.That(isEndOfTextPosition, Is.EqualTo(expectedIsEnd));
        }

        [Test]
        public void CompleteInput_Should_Run_As_Expected()
        {
            // arrange
            var history = new Mock<IInputHistory>();

            var state = new ConsoleState(history.Object);

            var text = "test text";
            state.Text.AppendLine(text);
            state.Prompt = "some test prompt";
            state.TextRowPosition = 123;

            var variables = Enumerable
                .Range(1, 3)
                .Select(i => $"test-variable-{i}")
                .ToList();

            // act
            state.CompleteInput(variables);

            // assert
            history.Verify(h => h.Add(text), "should add text to history and remove last line ending");
            Assert.That(state.Variables, Is.EquivalentTo(variables));
            Assert.That(state.Prompt, Is.EqualTo(Prompt.Standard));
            Assert.That(state.TextRowPosition, Is.Zero);
            Assert.That(state.Text.ToString(), Is.Empty);
        }
    }
}
