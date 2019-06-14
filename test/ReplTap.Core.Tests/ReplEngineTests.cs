using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
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
            const string expectedFirstOuput = null;
            const string expectedSecondOutput = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondOutput}\";";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
            // act
            var firstResult  = await engine.Execute(firstCode);
            var secondResult  = await engine.Execute(secondCode);
            
            // assert
            Assert.That(firstResult.Output, Is.EqualTo(expectedFirstOuput));
            Assert.That(firstResult.State, Is.EqualTo(OutputState.Valid));
            
            Assert.That(secondResult.Output, Is.EqualTo(expectedSecondOutput));
            Assert.That(secondResult.State, Is.EqualTo(OutputState.Valid));
        }
        
        [Test]
        public void Execute_Should_Throw_Exception_When_Run_Invalid_Code_()
        {
            // arrange
            var invalidCode = "var";
            
            var engine = new ReplEngine();
            
            // act && assert
            Assert.ThrowsAsync<CompilationErrorException>(async () => await engine.Execute(invalidCode));
        }
        
        
        [Test]
        public async Task Execute_Should_Continue_With_Same_State_After_Throwing_Exception()
        {
            // arrange
            const string expectedFirstResult = null;
            const string expectedSecondResult = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondResult}\";";
            var invalidCode = "var";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
            // act && assert
            var firstResult  = await engine.Execute(firstCode);
            Assert.ThrowsAsync<CompilationErrorException>(async () => await engine.Execute(invalidCode));
            var secondResult  = await engine.Execute(secondCode);
            
            // assert
            Assert.That(firstResult.Output, Is.EqualTo(expectedFirstResult));
            Assert.That(firstResult.State, Is.EqualTo(OutputState.Valid));
            
            Assert.That(secondResult.Output, Is.EqualTo(expectedSecondResult));
            Assert.That(secondResult.State, Is.EqualTo(OutputState.Valid));
        }
    }
}