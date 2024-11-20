using NUnit.Framework;
using SyncTask.Utilities;

namespace SyncTaskTests.Utilities
{
    [TestFixture]
    public class HashUtilsTest
    {

        #region Tests for method AreHashesEqual() 

        [TestCase("abcd12345", "abcd12345")]
        [TestCase("aBcD12345", "aBcD12345")]
        public void AreHashesEqual_ShouldReturnTrue_ForEqualHashes(string a, string b)
        {
            bool result = HashUtils.AreHashesEqual(a, b);
            Assert.IsTrue(result);
        }

        [TestCase("abcd12345", "abcd123")]
        [TestCase("aBCD12345", "abcd12345")]
        [TestCase("abcd12345", "aBCD12345")]
        [TestCase("", "abcd12345")]
        [TestCase(null, "abcd12345")]
        [TestCase("abcd12345", null)]
        [TestCase("abcd12345", "")]
        public void AreHashesEqual_ShouldReturnFalse_ForNotEqualHashes(string a, string b)
        {
            bool result = HashUtils.AreHashesEqual(a, b);
            Assert.IsFalse(result);
        }

        #endregion
    }
}
