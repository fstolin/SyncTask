using SyncTask.Structs;
using System.Globalization;

namespace SyncTask.Utilities
{
    public static class Utils
    {
        public static float TryConvertToFloat(string value)
        {
            try
            {
                value = value.Replace(",", ".");
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("[Error] Object cannot be converted to float.");
                throw new FormatException();
            }
        }

        public static bool AreArgumentsValid(Arguments args)
        {
            if (args.Interval == 0 || args.Interval < 0)
            {
                Console.Write("Interval must be greater than 0! ");
                return false;
            }
            else if (args.SourcePath.ToLower() == args.TargetPath.ToLower())
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
    }
}
