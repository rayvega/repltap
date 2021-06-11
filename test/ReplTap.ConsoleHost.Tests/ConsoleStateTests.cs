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
        [TestCase("", "")]
        [TestCase("", "old test current line")]
        [TestCase("test current line 1\n", "old test current line 2")]
        [TestCase("test current line 1\nold test current line 2\n", "old test current line 3")]
        public void CurrentLineText_Should_Set_Expected_When_Last_Line(string startText, string endText)
        {
            // arrange
            var state = new ConsoleState(new InputHistory());

            state.Text.Append($"{startText}{endText}");

            Assert.That(state.CurrentLineText, Is.EqualTo(endText));

            var expectedCurrentLineText = "new current test line";

            // act
            state.CurrentLineText = expectedCurrentLineText;

            // assert
            var line = state.CurrentLineText;

            Assert.That(line, Is.EqualTo(expectedCurrentLineText));
            Assert.That(state.Text.ToString(), Is.EqualTo($"{startText}{expectedCurrentLineText}"));
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
    }
}
