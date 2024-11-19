namespace SyncTask.Utilities
{
    public static class Utils
    {
        public static float TryConvertToFloat(object value)
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
    }
}
