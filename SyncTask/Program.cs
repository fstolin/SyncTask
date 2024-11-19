using SyncTask.ArgumentHandling;
using SyncTask.Services;
using SyncTask.Exceptions;

namespace SyncTask
{
    internal class Program
    {

        static void Main(string[] args)
        {
            try
            {
                ArgumentHandler argumentHandler = new ArgumentHandler(args);
                SyncRuntimeManager syncRuntimeManager = new SyncRuntimeManager(argumentHandler.GetValidArguments());
                syncRuntimeManager.StartSyncing();
            }
            catch (InvalidCmdParametersException)
            {
                Console.WriteLine("Error starting the synchronization. Press any key to exit.");
            }
        }
    }
}
