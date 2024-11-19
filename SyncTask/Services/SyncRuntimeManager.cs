using SyncTask.Logging;
using SyncTask.Structs;
using SyncTask.Utils;

namespace SyncTask.Services
{
    public class SyncRuntimeManager
    {

        private readonly Arguments args;

        // Is it better to have replication manager as a field or passs it in the methods?
        private ReplicationManager? replicationManager;

        public SyncRuntimeManager(Arguments args)
        {
            this.args = args;
        }

        public void StartSyncing()
        {
            SetupDependencies();

            while (true)
            {
                // Check if a key has been pressed
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break; 
                }
                replicationManager!.InitializeReplication();
                Thread.Sleep(Convert.ToInt32(args.Interval*1000));
            }
        }

        private void SetupDependencies()
        {
            LoggingManager logManager = new LoggingManager(args.LogFilePath);
            replicationManager = new ReplicationManager(args.SourcePath, args.TargetPath);
            replicationManager.LogMessageSent += logManager.OnLogEventRaised;
            HashUtils.UtilsLogMessageSent += logManager.OnLogEventRaised;
        }

    }
}
