using SyncTask.ReplicationManagement;

namespace SyncTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? sourcePath = Console.ReadLine();
            if (sourcePath == null) return;
            ReplicationManager repManager = new ReplicationManager(sourcePath, "", 1.0f);
            repManager.InitializeReplication();

            Console.WriteLine("Program finished. Press any key to exit.");
            Console.ReadKey();
        }

    }
}
