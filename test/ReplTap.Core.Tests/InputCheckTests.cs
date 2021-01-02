using NUnit.Framework;

namespace ReplTap.Core.Tests
{
    [TestFixture]
    public class InputCheckTests
    {
        private static object[] _testCasesNewLine =
        {
            new object[] {"test code line 1 \r\r\r", true},
            new object[] {"test code line 1\r\r\r", true},
            new object[] {"test code line 1\rtest code line 2\r\r\r", true},
            new object[] {"test code line 1", false},
            new object[] {"test code line 1 \r", false},
            new object[] {"test code line 1 \ra\r", false},
            new object[] {"test code line 1 \r \r", false},
            new object[] {"test code line 1 \r\ra", false},
            new object[] {"test code line 1 \ra\ra", false},
        };

        [TestCaseSource(nameof(_testCasesNewLine))]
        public void IsForceExecute_Multiple_Newlines_Should_Return_Expected(string input, bool expectedIsForceExecute)
        {
            var inputCheck = new InputCheck();

            var isForceExecute = inputCheck.IsForceExecute(input);

            Assert.That(isForceExecute, Is.EqualTo(expectedIsForceExecute));
        }
    }
}