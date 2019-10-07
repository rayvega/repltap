using NUnit.Framework;

namespace ReplTap.Core.Tests
{
    [TestFixture]
    public class InputCheckTests
    {
        [Test]
        [TestCase("test code;", true)]
        [TestCase(";test code", true)]
        [TestCase("test ; code", true)]
        [TestCase(";", true)]
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