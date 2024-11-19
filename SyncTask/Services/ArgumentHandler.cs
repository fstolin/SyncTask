using SyncTask.Structs;
using SyncTask.Exceptions;

namespace SyncTask.ArgumentHandling
{

    public class ArgumentHandler
    {

        private string[] args;

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
                TryConvertToFloat(args[3]));

            if (AreArgumentsValid(arguments))
            {
                return arguments;
            }
            else
            {
                throw new InvalidCmdParametersException();
            }
        }

        private bool AreArgumentsValid(Arguments args)
        {
            if (ArePathsValid(args) && args.Interval != 0)
            {
                return true;
            }
            return false;
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

        private bool ArePathsValid(Arguments args)
        {

            if (args.SourcePath.ToLower() == args.TargetPath.ToLower())
            {
                Console.Write("Source path and target path are the same! ");
                return false;
            }
            else if (args.LogFilePath.StartsWith(args.TargetPath))
            {
                Console.Write("Log path is in target path's directory! ");
                return false;
            }
            return true;
        }

        // TODO move to utils
        private float TryConvertToFloat(object value)
        {
            try
            {
                return Convert.ToSingle(value);
            }
            catch (FormatException) 
            {
                Console.WriteLine("[Error] Object cannot be converted to float.");
                throw new FormatException();
            }
        }

        // TODO move to utils
        private static void TerminateProgram()
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
