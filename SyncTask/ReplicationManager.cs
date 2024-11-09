using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.ReplicationManagement
{
    class ReplicationManager
    {
        private string InitialSourcePath;
        private string TargetPath;
        private float SyncInterval;

        public ReplicationManager(string sourcePath, string targetPath, float Interval) 
        {
            InitialSourcePath = sourcePath;
            TargetPath = targetPath;
            SyncInterval = Interval;
        }

        // Initializes the replication process
        public void InitializeReplication()
        {
            TraverseDirectory(InitialSourcePath);
        }

        // Recursively goes through files and subfolders in selectedPath
        private void TraverseDirectory(string sourcePath)
        {            
            try
            {
                string[] subFolders = Directory.GetDirectories(sourcePath);
                // Traversing  all files
                foreach (string file in Directory.GetFiles(sourcePath))
                {
                    Console.WriteLine(file);
                }

                // Traversing  all folders
                foreach (string subFolder in subFolders)
                {
                    Console.WriteLine(subFolder);
                    // Traverse next subfolder
                    TraverseDirectory(subFolder);
                }
            } 
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The source directory doesn't exist. Please try again.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied. Please try again.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
            }
        }
    }
}
