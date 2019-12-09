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
    }
}