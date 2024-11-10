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
        private string LogFilePath;
        private float SyncInterval;

        public ReplicationManager(string sourcePath, string targetPath, string logFilePath, float Interval) 
        {
            InitialSourcePath = sourcePath;
            TargetPath = targetPath;
            LogFilePath = logFilePath;
            SyncInterval = Interval;
        }

        // Initializes the replication process
        public void InitializeReplication()
        {
            try
            {
                Directory.CreateDirectory(TargetPath);
                TraverseDirectory(InitialSourcePath);
            }
            catch(IOException) {
                Console.WriteLine("I/O error. Please try again.");
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

        // Recursively goes through files and subfolders in selectedPath
        private void TraverseDirectory(string sourcePath)
        {
            try
            {
                string[] subFolders = Directory.GetDirectories(sourcePath);
                string[] files = Directory.GetFiles(sourcePath);

                // Traversing all folders
                foreach (string subFolder in subFolders)
                {
                    Directory.CreateDirectory(GetRelativeTargetPath(subFolder));

                    // Traverse next subfolder
                    TraverseDirectory(subFolder);
                }

                // Traversing all files
                foreach (string file in files)
                {
                    string newPath = GetRelativeTargetPath(file);
                    File.Copy(file, newPath, true);
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The source directory doesn't exist. Please try again.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File has not been found during replication.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
            }
        }

        // Converts from original path to a relative path relative to the target directory
        private string GetRelativeTargetPath(string path)
        {
            string relativePath = Path.GetRelativePath(InitialSourcePath, path);
            return Path.Combine(TargetPath, relativePath);
        }
    }
}
