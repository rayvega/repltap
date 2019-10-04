using NUnit.Framework;

namespace ReplTap.ConsoleHost.Tests
{
    [TestFixture]
    public class InputCheckTests
    {
        [Test]
        // valid
        [TestCase("test code;", true)]
        [TestCase(";test code", true)]
        [TestCase("test ; code", true)]
        [TestCase(";", true)] 

        // invalid
        [TestCase("", false)] 
        [TestCase(null, false)] 
        [TestCase("test code", false)] 
        [TestCase("", false)] 
        public void IsForceExecute_Should_Return_As_Expected(string input, bool expectedIsForceExecute)
        {
            var inputCheck = new InputCheck();

            var isForceExecute = inputCheck.IsForceExecute(input);

            Assert.That(isForceExecute, Is.EqualTo(expectedIsForceExecute));
        }
    }
}