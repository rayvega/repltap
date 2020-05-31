using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace ReplTap.Core.Tests
{
    [TestFixture]
    public class ReplEngineTests
    {
        [Test]
        public async Task Execute_Should_Return_Valid_State_When_Single_Input()
        {
            // arrange
            var expectedOutput = "test output";
            var expectedVariable = "testVariable";

            var code = $"var {expectedVariable} = \"{expectedOutput}\"; testVariable";
            var expectedVariables = new[] {expectedVariable};

            var inputCheck = new Mock<IInputCheck>();

            var engine = new ReplEngine(inputCheck.Object);

            // act
            var result  = await engine.Execute(code);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Output, Is.EqualTo(expectedOutput));
            Assert.That(result.State, Is.EqualTo(OutputState.Valid));
            Assert.That(result.Variables, Is.SupersetOf(expectedVariables));
        }

        [Test]
        public async Task Execute_Should_Return_Valid_State_When_Two_Inputs()
        {
            // arrange
            const string expectedFirstOutput = null;
            const string expectedSecondOutput = "test value";

            var firstCode = $"var testVariable = \"{expectedSecondOutput}\";";
            var secondCode = "testVariable";
            var inputCheck = new Mock<IInputCheck>();

            var engine = new ReplEngine(inputCheck.Object);

            // act
            var firstResult  = await engine.Execute(firstCode);
            var secondResult  = await engine.Execute(secondCode);

            // assert
            Assert.That(firstResult.Output, Is.EqualTo(expectedFirstOutput));
            Assert.That(firstResult.State, Is.EqualTo(OutputState.Valid));

            Assert.That(secondResult.Output, Is.EqualTo(expectedSecondOutput));
            Assert.That(secondResult.State, Is.EqualTo(OutputState.Valid));
        }

        [Test]
        public async Task Execute_Should_Return_Continue_State_When_Invalid_Input()
        {
            // arrange
            const string expectedFirstResult = null;
            const string expectedSecondResult = "test value";

            var firstCode = $"var testVariable = \"{expectedSecondResult}\";";
            var invalidCode = "invalid code;";
            var secondCode = "testVariable";
            var inputCheck = new Mock<IInputCheck>();

            var engine = new ReplEngine(inputCheck.Object);

            // act
            var firstResult  = await engine.Execute(firstCode);
            var invalidResult = await engine.Execute(invalidCode);
            var secondResult  = await engine.Execute(secondCode);

            // assert
            Assert.That(firstResult.Output, Is.EqualTo(expectedFirstResult));
            Assert.That(firstResult.State, Is.EqualTo(OutputState.Valid));

            Assert.That(invalidResult.Output, Is.EqualTo(null));
            Assert.That(invalidResult.State, Is.EqualTo(OutputState.Continue));

            Assert.That(secondResult.Output, Is.EqualTo(expectedSecondResult));
            Assert.That(secondResult.State, Is.EqualTo(OutputState.Valid));
        }

        [Test]
        public async Task Execute_Should_Continue_When_Not_Complete()
        {
            // arrange
            var codeLines = new List<string>
            {
                "var ",
                "foo =\"bar\";",
                "foo",
            };

            var inputCheck = new Mock<IInputCheck>();
            var engine = new ReplEngine(inputCheck.Object);
            var builder = new StringBuilder();
            var codeResult = new CodeResult();

            // act

            foreach (var line in codeLines)
            {
                builder.AppendLine(line);

                codeResult = await engine.Execute(builder.ToString());
            }

            // assert
            Assert.That(codeResult.Output, Is.EqualTo("bar"));
            Assert.That(codeResult.State, Is.EqualTo(OutputState.Valid));
        }

        [Test]
        public async Task Execute_Should_Return_Error_State_When_Invalid_Input_And_Force_Execute()
        {
            // arrange
            var input = "invalid code with semicolon to force execute ; ";
            var inputCheck = new Mock<IInputCheck>();
            var engine = new ReplEngine(inputCheck.Object);

            inputCheck
                .Setup(i => i.IsForceExecute(input))
                .Returns(true);

            // act
            var codeResult = await engine.Execute(input);

            // assert
            Assert.That(codeResult.Output, Is.EqualTo("(1,14): error CS1002: ; expected"));
            Assert.That(codeResult.State, Is.EqualTo(OutputState.Error));
        }
    }
}