using NUnit.Framework;
using static System.Environment;

namespace ReplTap.Core.Tests
{
    [TestFixture]
    public class InputCheckTests
    {
        private static object[] _testCasesNewLine =
        {
            new object[] { $"test code line 1 {NewLine}{NewLine}{NewLine}", true },
            new object[] { $"test code line 1{NewLine}{NewLine}{NewLine}", true },
            new object[] { $"test code line 1{NewLine}test code line 2{NewLine}{NewLine}{NewLine}", true },
            new object[] { "test code line 1", false },
            new object[] { $"test code line 1 {NewLine}", false },
            new object[] { $"test code line 1 {NewLine}a{NewLine}", false },
            new object[] { $"test code line 1 {NewLine} {NewLine}", false },
            new object[] { $"test code line 1 {NewLine}{NewLine}a", false },
            new object[] { $"test code line 1 {NewLine}a{NewLine}a", false },
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
