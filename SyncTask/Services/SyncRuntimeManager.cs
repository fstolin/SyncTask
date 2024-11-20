using SyncTask.Interfaces;
using SyncTask.Logging;
using SyncTask.Structs;
using SyncTask.Utilities;

namespace SyncTask.Services
{
    public class SyncRuntimeManager
    {

        private readonly Arguments args;
        private readonly IUserInputChecker inputChecker;

        // Is it better to have replication manager as a field or passs it in the methods?
        private ReplicationManager? replicationManager;

        public SyncRuntimeManager(Arguments args, IUserInputChecker inputChecker)
        {
            this.args = args;
            this.inputChecker = inputChecker;
        }

        public void StartSyncing()
        {
            SetupDependencies();

            while (!inputChecker.UserPressedKey())
            {
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
