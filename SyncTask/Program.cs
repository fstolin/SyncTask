using SyncTask.ReplicationManagement;
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
            float interval = 2.0f;

            replicationManager = new ReplicationManager(sourcePath, targetPath, interval);
            LoggingManager logManager = new LoggingManager(logFilePath);
            replicationManager.OnItemChanged += logManager.OnFileChangedEvent;

            StartSyncTimer(interval);

            if (initSuccesful)
            {
                Console.WriteLine("Synchronization is active. Press any key to exit.");
            } 
            else
            {
                Console.WriteLine("Press any key to exit.");
            }

            Console.ReadKey();
            
        }

        private static void OnSyncIntervalElapsed(object? source, ElapsedEventArgs e)
        {
            replicationManager.InitializeReplication();
            Console.WriteLine($"[{e.SignalTime.ToString("HH:mm:ss")}] Folders have been synchronized.");
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
