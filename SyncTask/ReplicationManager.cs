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
        private readonly string InitialSourcePath;
        private readonly string TargetPath;
        private readonly string LogFilePath;
        private readonly float SyncInterval;
        private Dictionary<string, string> SourceFilesDictionary;

        public ReplicationManager(string sourcePath, string targetPath, string logFilePath, float Interval) 
        {
            InitialSourcePath = sourcePath;
            TargetPath = targetPath;
            LogFilePath = logFilePath;
            SyncInterval = Interval;
            SourceFilesDictionary = new Dictionary<string, string>();
        }

        // Initializes the replication process
        public void InitializeReplication()
        {
            try
            {
                // Temporary hack
                SourceFilesDictionary.Clear();
                Directory.CreateDirectory(TargetPath);
                ReplicateDirectory(InitialSourcePath);
                CleanUpReplica(TargetPath);
            }
            catch(IOException) {
                Console.WriteLine("I/O error. Please try again.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied during initialization. Please try again.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
                throw;
            }
            // Initialization was succesful, synchronization is in progress.
            Program.SetInitSuccesful();
        }

        // Recursively goes through files and subfolders in selectedPath.
        private void ReplicateDirectory(string sourcePath)
        {

            try
            {
                string[] subFolders = Directory.GetDirectories(sourcePath);
                string[] files = Directory.GetFiles(sourcePath);

                // Traversing all folders
                foreach (string subFolder in subFolders)
                {
                    Directory.CreateDirectory(GetAbsoluteTargetPath(subFolder));
                    SourceFilesDictionary.Add(subFolder, Path.GetFileName(subFolder));

                    // Traverse next subfolder
                    ReplicateDirectory(subFolder);
                }

                // Traversing all files
                foreach (string file in files)
                {
                    string newFilePath = GetAbsoluteTargetPath(file);
                    File.Copy(file, newFilePath, true);
                    SourceFilesDictionary.Add(file, Path.GetFileName(file));
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
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied. Please try again.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
                throw;
            }
        }

        // Deletes files, which are not in the source directory (dictionary).
        private void CleanUpReplica(string targetPath)
        {
            try
            {
                string[] subFolders = Directory.GetDirectories(targetPath);
                string[] files = Directory.GetFiles(targetPath);

                // Traversing all folders
                foreach (string subFolder in subFolders)
                {
                    if (shouldBeRemoved(subFolder))
                    {
                        Directory.Delete(subFolder, true);
                    } else
                    {
                        CleanUpReplica(subFolder);
                    }
                }

                 //Traversing all files
                foreach (string file in files)
                {
                    if (shouldBeRemoved(file))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The source directory doesn't exist during cleanup. Please try again.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File has not been found during cleanup.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied during cleanup. Please try again.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
            }
        }

        // Returns true if the file is missing from source dictionary, meaning it should be deleted
        private bool shouldBeRemoved(string targetAbsolutePath)
        {
            if (SourceFilesDictionary.ContainsKey(GetAbsoluteSourcePath(targetAbsolutePath)))
            {
                return false;
            }
            return true;
        }

        // Converts from original absolute path to absolute path for the target directory
        private string GetAbsoluteTargetPath(string path)
        {
            string relativePath = Path.GetRelativePath(InitialSourcePath, path);
            return Path.Combine(TargetPath, relativePath);
        }

        // Converts from target absolute path to original absolute path
        private string GetAbsoluteSourcePath(string path)
        {
            string relativePath = Path.GetRelativePath(TargetPath, path);
            return Path.Combine (InitialSourcePath, relativePath);
        }
    }
}
