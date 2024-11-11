using SyncTask.Logging;
using SyncTask.ReplicationManagement;
using SyncTask.Utils;
using System.Timers;

namespace SyncTask
{
    internal class Program
    {
        private static ReplicationManager? replicationManager;
        private static LoggingManager? logManager;
        private static string? sourcePath;
        private static string? targetPath;
        private static string? logFilePath;
        private static float interval = 10.0f;
        private static System.Timers.Timer syncTimer = new System.Timers.Timer();
        public static bool isSyncing { get; set; }

        static void Main(string[] args)
        {
            HandleArguments(args);

            if (sourcePath == null || targetPath == null || logFilePath == null)
            {
                Console.Write("At least one of the paths is null! ");
                TerminateProgram();
                return;
            } 
            if (!ArePathsValid(sourcePath, targetPath, logFilePath))
            {
                TerminateProgram();
                return;
            }

            // Start the main flow - create logManager,
            // create replication manager, subscribe to logging events.
            logManager = new LoggingManager(logFilePath);
            replicationManager = new ReplicationManager(sourcePath, targetPath);
            replicationManager.LogMessageSent += logManager.OnLogEventRaised;
            HashUtils.UtilsLogMessageSent += logManager.OnLogEventRaised;

            // Start the synchronization and synchronization timer
            replicationManager.InitializeReplication();
            StartSyncTimer(interval);
            Console.ReadKey();
        }

        private static void OnSyncIntervalElapsed(object? source, ElapsedEventArgs e)
        {
            if (!isSyncing)
            {
                replicationManager?.InitializeReplication();
            }
        }

        private static void StartSyncTimer(float interval)
        {
            syncTimer.Interval = interval * 1000;
            syncTimer.Elapsed += OnSyncIntervalElapsed;            
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
        }

        // Checks whether the paths (source, replica, log) are valid.
        // Uses console writing, logManager is instanced after check.
        private static bool ArePathsValid(string source, string target, string log)
        {
            if (source.ToLower() == target.ToLower())
            {
                Console.Write("Source path and target path are the same! ");
                return false;
            }
            else if (log.StartsWith(target))
            {
                Console.Write("Log path is in target path's directory! ");
                return false;
            }
            return true;
        }

        private static void TerminateProgram()
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void HandleArguments(string[] args)
        {
            // Check number of arguments
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("[Error] Incorrect number of arguments. Expected 3 or 4. Correct format: (source path, target path, log file path, interval [s] (default: 10s)");
                TerminateProgram();
                return;
            }

            // Assign arguments to their variables
            sourcePath = args[0];
            targetPath = args[1];
            logFilePath = args[2];
            if (args.Length > 3)
            {
                interval = Convert.ToSingle(args[3]);
            }
        }

    }
}
