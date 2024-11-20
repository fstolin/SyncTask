
using NUnit.Framework;
using SyncTask.ArgumentHandling;
using SyncTask.Exceptions;
using SyncTask.Structs;

namespace SyncTaskTests.Services
{

    [TestFixture]
    public class ArgumentHandlerTests
    {

        #region Tests for method GetValidArguments

        private static IEnumerable<object> GetValidArguments_ShouldReturnValidArguments_ForValidInput_Input()
        {
            return
            [
                // 4 arguments
                new object[] {
                    new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "5.84"}, 
                    new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 5.84f)
                },
                // 4 arguments
                new object[] {
                    new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "5,84"}, 
                    new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 5.84f)
                },
                // 4 arguments
                new object[] {
                    new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "60"},
                    new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 60)
                },
                // 3 arguments
                new object[] {
                    new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt"},
                    new Arguments("D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", 10)
                },
            ];
        }

        [TestCaseSource(nameof(GetValidArguments_ShouldReturnValidArguments_ForValidInput_Input))]
        public void GetValidArguments_ShouldReturnValidArguments_ForValidInput(string[] args, Arguments expectedArguments)
        {
            ArgumentHandler argumentHandler = new ArgumentHandler(args);
            Arguments resultArguments = argumentHandler.GetValidArguments();

            Assert.AreEqual(expectedArguments, resultArguments);
        }

        private static IEnumerable<object> GetValidArguments_ShouldThrowAnException_ForInvalidInput_Input()
        {
            return
            [
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "5.84", "AnotherPath"}, // More arguments
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup"}, // Less arguments
                new string[] { "D:\\Sandbox\\Temp"}, // Less arguments
                Array.Empty<string>(), // No Arguments
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Temp", "D:\\Sandbox\\log.txt", "5.84"}, // Same source 
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "-5.84"}, // Negative interval
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\log.txt", "0"}, // 0 Interval
                new string[] { "D:\\Sandbox\\Temp", "D:\\Sandbox\\Backup", "D:\\Sandbox\\Backup\\log.txt", "5.84"}, // Log inside target
            ];
        }

        [TestCaseSource(nameof(GetValidArguments_ShouldThrowAnException_ForInvalidInput_Input))]
        public void GetValidArguments_ShouldThrowAnException_ForInvalidInput(string[] args)
        {
            ArgumentHandler argumentHandler = new ArgumentHandler(args);

            Assert.Throws<InvalidCmdParametersException>(() => argumentHandler.GetValidArguments());
        }

        #endregion
    }
}
