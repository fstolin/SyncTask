using NUnit.Framework;
using System.Globalization;


namespace SyncTask.Structs.Tests
{
    [TestFixture]
    internal class ArgumentsTests
    {

        #region Tests of ToString method

        [TestCaseSource(nameof(ToString_ShouldReturnCorrectString_ForInputData_Inputs))]
        public void ToString_ShouldReturnCorrectString_ForInputData(Arguments input, string expected)
        {
            string result = input.ToString();
            result = result.Replace(",",".");
            Assert.AreEqual(expected, result);
        }

        private static IEnumerable<object> ToString_ShouldReturnCorrectString_ForInputData_Inputs()
        {
            return
            [
                new object[] { new Arguments("SOURCE", "TARGET", "LOG", 5.48f), $"Source: SOURCE\nTarget: TARGET\nLogPath: LOG\nInterval: 5.48" },
                new object[] { new Arguments(string.Empty, string.Empty, string.Empty, 0.0f), $"Source: \nTarget: \nLogPath: \nInterval: 0" }
            ];
        }

        #endregion
    }
}
