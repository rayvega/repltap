using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ReplTap.Core.Tests
{
    [TestFixture]
    public class ReplEngineTests
    {
        [Test]
        public async Task Execute_Should_Run_Code_On_Initial()
        {
            // arrange
            var expectedOutput = "test output";
            var code = $"var testVariable = \"{expectedOutput}\"; testVariable";
            
            var engine = new ReplEngine();
            
            // act
            var result  = await engine.Execute(code);
            
            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Output, Is.EqualTo(expectedOutput));
            Assert.That(result.State, Is.EqualTo(OutputState.Valid));
        }
        
        
        [Test]
        public async Task Execute_Should_Run_Code_On_Continue()
        {
            // arrange
            const string expectedFirstOutput = null;
            const string expectedSecondOutput = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondOutput}\";";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
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
        public async Task Execute_Should_Return_Continue_State_When_Invalid_Code()
        {
            // arrange
            const string expectedFirstResult = null;
            const string expectedSecondResult = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondResult}\";";
            var invalidCode = "invalid code;";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
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
        public async Task Execute_Should_Continue_If_Not_Complete()
        {
            // arrange
            var codeLines = new List<string>
            {
                "var ",
                "foo =\"bar\";",
                "foo",
            };
            
            var engine = new ReplEngine();
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
    }
}