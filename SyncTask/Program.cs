using SyncTask.ReplicationManagement;

namespace SyncTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: Change to cmd line parameters
            Console.WriteLine("Enter sourcePath:");
            string sourcePath = Console.ReadLine();
            string targetPath = "D:/Sandbox/Backup";
            //Console.WriteLine("Enter logFilePath:");
            string logFilePath = "empty";
            //Console.WriteLine("Enter interval:");
            float interval = 5.0f;

            ReplicationManager replicationManager = new ReplicationManager(sourcePath, targetPath, logFilePath, interval);
            replicationManager.InitializeReplication();
        }

    }
}
