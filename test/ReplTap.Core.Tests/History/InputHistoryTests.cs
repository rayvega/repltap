using System.Linq;
using NUnit.Framework;
using ReplTap.Core.History;

namespace ReplTap.Core.Tests.History
{
    [TestFixture]
    public class InputHistoryTests
    {
        [Test]
        public void GetPreviousInput_Should_Return_Last_Input([Range(1, 3)]int count)
        {
            // arrange
            var inputHistory = new InputHistory();

            var lastInput = "";
            foreach (var index in Enumerable.Range(1, count))
            {
                lastInput = $"test input # {index}";
                inputHistory.Add(lastInput);
            }

            // act
            var previousInput = inputHistory.GetPreviousInput();

            // assert
            Assert.That(previousInput, Is.EqualTo(lastInput));
        }

        [Test]
        [TestCase(1, "test input # 4")]
        [TestCase(2, "test input # 3")]
        [TestCase(3, "test input # 2")]
        [TestCase(4, "test input # 1")]
        [TestCase(5, "")]
        [TestCase(6, "")]
        [TestCase(7, "")]
        public void GetPreviousInput_Should_Return_Previous_Input(int count, string expectedInput)
        {
            // arrange
            var inputHistory = new InputHistory();

            foreach (var index in Enumerable.Range(1, 4))
            {
                var input = $"test input # {index}";
                inputHistory.Add(input);
            }

            // act
            var previousInput = "";

            foreach (var _ in Enumerable.Range(1, count))
            {
                previousInput = inputHistory.GetPreviousInput();
            }

            // assert
            Assert.That(previousInput, Is.EqualTo(expectedInput));
        }

        [Test]
        public void Reset_Should_Go_To_Last_Input_When_Has_Any_History([Range(1, 4)] int count)
        {
            // arrange
            var expectedLastInput = "test input # 4";

            var inputHistory = new InputHistory();

            foreach (var index in Enumerable.Range(1, 4))
            {
                var input = $"test input # {index}";
                inputHistory.Add(input);
            }

            // act
            foreach (var _ in Enumerable.Range(1, count))
            {
                inputHistory.GetPreviousInput();
            }

            inputHistory.Reset();

            var previousInput = inputHistory.GetPreviousInput();

            // assert
            Assert.That(previousInput, Is.EqualTo(expectedLastInput));
        }
    }
}