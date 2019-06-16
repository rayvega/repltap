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
            var expectedReturnValue = "test value";
            var code = $"var testVariable = \"{expectedReturnValue}\"; testVariable";
            
            var engine = new ReplEngine();
            
            // act
            var returnValue  = await engine.Execute(code);
            
            // assert
            Assert.That(returnValue, Is.EqualTo(expectedReturnValue));
        }
        
        
        [Test]
        public async Task Execute_Should_Run_Code_On_Continue()
        {
            // arrange
            const string expectedFirstReturnValue = null;
            const string expectedSecondReturnValue = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondReturnValue}\";";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
            // act
            var firstReturnValue  = await engine.Execute(firstCode);
            var secondReturnValue  = await engine.Execute(secondCode);
            
            // assert
            Assert.That(firstReturnValue, Is.EqualTo(expectedFirstReturnValue));
            Assert.That(secondReturnValue, Is.EqualTo(expectedSecondReturnValue));
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
            const string expectedFirstReturnValue = null;
            const string expectedSecondReturnValue = "test value";
            
            var firstCode = $"var testVariable = \"{expectedSecondReturnValue}\";";
            var invalidCode = "var";
            var secondCode = "testVariable";
            
            var engine = new ReplEngine();
            
            // act && assert
            var firstReturnValue  = await engine.Execute(firstCode);
            Assert.ThrowsAsync<CompilationErrorException>(async () => await engine.Execute(invalidCode));
            var secondReturnValue  = await engine.Execute(secondCode);
            
            // assert
            Assert.That(firstReturnValue, Is.EqualTo(expectedFirstReturnValue));
            Assert.That(secondReturnValue, Is.EqualTo(expectedSecondReturnValue));
        }
    }
}