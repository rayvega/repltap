using System;
using System.Linq;
using NUnit.Framework;
using ReplTap.Core.History;

namespace ReplTap.Core.Tests.History
{
    [TestFixture]
    public class InputHistoryTests
    {
        [Test]
        [TestCase(6, "")]
        [TestCase(5, "")]
        [TestCase(4, "test input # 0")]
        [TestCase(3, "test input # 1")]
        [TestCase(2, "test input # 2")]
        [TestCase(1, "test input # 3")]
        public void GetPreviousInput_Should_Return_Previous_Input(int count, string expectedInput)
        {
            // arrange
            var inputHistory = new InputHistory();

            foreach (var index in Enumerable.Range(0, 4))
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
        [TestCase(4, 1, "test input # 1")]
        [TestCase(4, 2, "test input # 2")]
        [TestCase(4, 4, "")]
        [TestCase(3, 2, "test input # 3")]
        [TestCase(2, 1, "test input # 3")]
        [TestCase(1, 1, "")]
        [TestCase(1, 2, "")]
        [TestCase(0, 1, "")]
        [TestCase(0, 2, "")]
        public void GetNextInput_Should_Return_Next_Input(int previousCount, int nextCount, string expectedInput)
        {
            // arrange
            var inputHistory = new InputHistory();

            foreach (var index in Enumerable.Range(0, 4))
            {
                var input = $"test input # {index}";
                inputHistory.Add(input);
                Console.WriteLine($"[debug] {input}");
            }

            // act
            foreach (var _ in Enumerable.Range(1, previousCount  ))
            {
                inputHistory.GetPreviousInput();
            }

            var nextInput = "";

            foreach (var _ in Enumerable.Range(1, nextCount))
            {
                nextInput = inputHistory.GetNextInput();
            }

            // assert
            Assert.That(nextInput, Is.EqualTo(expectedInput));
        }

        [Test]
        public void AllInputsAsString_Should_Return_Expected()
        {
            // arrange
            var expectedAllInputsAsString =
                $"test input 1{Environment.NewLine}test input 2{Environment.NewLine}test input 3";

            var inputHistory = new InputHistory();

            Enumerable
                .Range(1, 3)
                .ToList()
                .ForEach(i => inputHistory.Add($"test input {i}"));

            // act
            var allInputsAsString = inputHistory.AllInputsAsString();

            // assert
            Assert.That(allInputsAsString, Is.EqualTo(expectedAllInputsAsString));
        }
    }
}