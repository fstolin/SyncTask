namespace SyncTask.Exceptions
{
    public class InvalidCmdParametersException : Exception
    {
        public InvalidCmdParametersException() : base("Invalid command line parameters.") { }
    }

}
