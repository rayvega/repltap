using System.Linq;
using NUnit.Framework;
using ReplTap.Core.History;

namespace ReplTap.Core.Tests.History
{
    [TestFixture]
    public class InputHistoryTests
    {
        [Test]
        public void GetPreviousInput_Should_Return_Last_Input([Values(1, 2, 3)]int count)
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
    }
}