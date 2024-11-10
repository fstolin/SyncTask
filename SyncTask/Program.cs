using SyncTask.Logging;
using SyncTask.ReplicationManagement;
using SyncTask.Utils;
using System.Timers;

namespace SyncTask
{
    internal class Program
    {

        // Change to list to support multiple replications if needed
        private static ReplicationManager replicationManager;
        
        private static System.Timers.Timer syncTimer = new System.Timers.Timer();
        private static bool initSuccesful = false;

        public static void SetInitSuccesful() { initSuccesful = true; }

        static void Main(string[] args)
        {
            // TODO: Change to cmd line parameters
            Console.WriteLine("Enter sourcePath:");
            string sourcePath = Console.ReadLine();
            string targetPath = "D:\\Sandbox\\Backup";
            
            //Console.WriteLine("Enter logFilePath:");
            string logFilePath = "empty";
            //Console.WriteLine("Enter interval:");
            float interval = 10.0f;

            LoggingManager logManager = new LoggingManager(logFilePath);            

            if (sourcePath.ToLower() == targetPath.ToLower())
            {
                logManager.OnLogEventRaised(null, new LogEventArgs("Source path and target path are the same! Press any key to exit.", MessageType.Error));
                Console.ReadKey();
                return;
            }

            replicationManager = new ReplicationManager(sourcePath, targetPath);
            replicationManager.LogMessageSent += logManager.OnLogEventRaised;
            HashUtils.UtilsLogMessageSent += logManager.OnLogEventRaised;

            replicationManager.InitializeReplication();
            StartSyncTimer(interval);
            Console.ReadKey();
        }

        private static void OnSyncIntervalElapsed(object? source, ElapsedEventArgs e)
        {
            replicationManager.InitializeReplication();
        }

        private static void StartSyncTimer(float interval)
        {
            syncTimer.Interval = interval * 1000;
            syncTimer.Elapsed += OnSyncIntervalElapsed;            
            syncTimer.AutoReset = true;
            syncTimer.Enabled = true;
        }

    }
}
