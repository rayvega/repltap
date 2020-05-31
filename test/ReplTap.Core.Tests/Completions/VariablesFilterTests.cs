using System.Linq;
using Moq;
using NUnit.Framework;
using ReplTap.Core.Completions;

namespace ReplTap.Core.Tests.Completions
{
    [TestFixture]
    public class VariablesFilterTests
    {
        [Test]
        public void Filter_Should_Return_Expected()
        {
            // arrange
            var variables = Enumerable
                .Range(1, 3)
                .Select(i => $"variable {i}")
                .ToList();

            var code = "test code";

            var expectedFilteredVariables = Enumerable
                .Range(1, 3)
                .Select(i => $"filtered variable {i}")
                .ToList();

            var parser = new Mock<ICompletionsParser>();

            var filter = new Mock<ICompletionsFilter>();
            filter
                .Setup(f => f.Apply(variables, It.IsAny<string>()))
                .Returns(expectedFilteredVariables);

            var variablesFilter = new VariablesFilter(parser.Object, filter.Object);

            // act
            var filteredVariables = variablesFilter.Filter(code, variables);

            // assert
            Assert.That(filteredVariables, Is.EqualTo(expectedFilteredVariables));
        }
    }
}
