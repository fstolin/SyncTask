using SyncTask.Structs;
using SyncTask.Exceptions;
using SyncTask.Utilities;

namespace SyncTask.ArgumentHandling
{

    public class ArgumentHandler
    {

        private const float DefaultSyncInterval = 10.0f;
        private readonly string[] args;

        public ArgumentHandler(string[] args)
        {
            this.args = args;
        }

        // Could be written better, how?
        // How to separate validty check and returning of a struct?
        public Arguments GetValidArguments()
        {
            if (!IsNumberOfArgumentsValid())
            {
                throw new InvalidCmdParametersException();
            }

            Arguments arguments = new Arguments(
                args[0],
                args[1],
                args[2],
                GetFinalSyncInterval());

            if (Utils.AreArgumentsValid(arguments))
            {
                return arguments;
            }
            else
            {
                throw new InvalidCmdParametersException();
            }
        }

        private bool IsNumberOfArgumentsValid()
        {
            // Check number of arguments
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("[Error] Incorrect number of arguments. Expected 3 or 4. Correct format: (source path, target path, log file path, interval [s] (default: 10s)");
                return false;
            }
            return true;
        }

        private float GetFinalSyncInterval()
        {
            if (args.Length == 4)
            {
                return Utils.TryConvertToFloat(args[3]);
            }
            else
            {
                return DefaultSyncInterval;
            }
        }
    }
}
