
using NUnit.Framework;
using SyncTask.Exceptions;
using SyncTask.Structs;
using SyncTask.Utilities;

namespace SyncTaskTests;

[TestFixture]
public class UtilsTests
{
    [TestCase("5", 5f)]
    [TestCase("3.48", 3.48f)]
    [TestCase("5.00000006", 5.00000006f)]
    [TestCase("5,48", 5.48f)]
    public void TryConvertToFloat_ShouldReturnCorrectFloat_ForValidNumbers(string input, float expected)
    {
        // Arrange
        // Act
        float result = Utils.TryConvertToFloat(input);
        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestCase("String Input")]
    [TestCase("4.74f")]
    [TestCase("")]
    public void TryConvertToFloat_ShouldThrowAnException_ForInvalidInput(string input)
    {
        Assert.Throws<FormatException>(() => Utils.TryConvertToFloat(input));
    }

    [TestCaseSource(nameof(AreArgumentsValid_ValidTestCases))]
    public void AreArgumentsValid_ShouldReturnTrue_ForValidArguments(Arguments arguments)
    {
        bool result = Utils.AreArgumentsValid(arguments);
        Assert.IsTrue(result);
    }
    
    [TestCaseSource(nameof(AreArgumentsValid_InvalidTestCases))]
    public void AreArgumentsValid_ShouldReturnFalse_ForInvalidArguments(Arguments arguments)
    {
        bool result = Utils.AreArgumentsValid(arguments);
        Assert.IsFalse(result);
    }

    // TEST CASE SOURCES
    private static IEnumerable<object> AreArgumentsValid_ValidTestCases()
    {
        return
        [
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 5) },
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 5.48f) }
        ];
    }

    private static IEnumerable<object> AreArgumentsValid_InvalidTestCases()
    {
        return 
        [
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\sandbox\\Temp", "D:\\Sandbox\\log.txt", 5) },    // Same source & target folder
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", -8) },  // Negative interval
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", -8.42f) },  // Negative interval
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 0) },  // Interval == 0
            new object[] { new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\Backup\\Logging\\log.txt", 5) }    // Log inside target folder
        ];
    }

}
