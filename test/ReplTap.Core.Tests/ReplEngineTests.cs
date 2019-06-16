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
            var expectedReturnValue = "test value";
            var code = $"var testVariable = \"{expectedReturnValue}\"; testVariable";
            
            var engine = new ReplEngine();
            
            // act
            var returnValue  = await engine.Execute(code);
            
            // assert
            Assert.That(returnValue, Is.EqualTo(expectedReturnValue));
        }
    }
}