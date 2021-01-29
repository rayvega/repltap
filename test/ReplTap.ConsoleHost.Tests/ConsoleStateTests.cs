using NUnit.Framework;
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
            var state = new ConsoleState();
            state.Text.Append($"test code line 1{NewLine}test code line 2{NewLine}test code line 3");

            // act
            var lines = state.TextSplitLines;

            // assert
            Assert.That(lines.Length, Is.EqualTo(3));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("", "old test current line")]
        [TestCase("test current line 1\n", "old test current line 2")]
        [TestCase("test current line 1\nold test current line 2\n", "old test current line 3")]
        public void CurrentLineText_Should_Return_Expected(string startText, string endText)
        {
            // arrange
            var state = new ConsoleState();

            var expectedText = $"{startText}{endText}";
            state.Text.Append(expectedText);

            // act
            var line = state.CurrentLineText;

            // assert
            Assert.That(line, Is.EqualTo(endText));
            Assert.That(state.Text.ToString(), Is.EqualTo(expectedText));
        }


        [Test]
        [TestCase("", "")]
        [TestCase("", "old test current line")]
        [TestCase("test current line 1\n", "old test current line 2")]
        [TestCase("test current line 1\nold test current line 2\n", "old test current line 3")]
        public void CurrentLineText_Should_Set_Expected(string startText, string endText)
        {
            // arrange
            var state = new ConsoleState();

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
    }
}