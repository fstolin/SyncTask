using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            catch (NotSupportedException)
            {
                Console.WriteLine("Invalid target directory path. Please try again.");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Access denied in initialization.");
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
            string[] subFolders = Directory.GetDirectories(sourcePath);
            string[] files = Directory.GetFiles(sourcePath);

            // Traversing all folders
            foreach (string subFolder in subFolders)
            {
                HandleDirectoryReplication(subFolder);
                SourceFilesDictionary.Add(subFolder, Path.GetFileName(subFolder));
                // Traverse next subfolder
                ReplicateDirectory(subFolder);
            }

            // Traversing all files
            foreach (string file in files)
            {
                HandleFileReplication(file);
                SourceFilesDictionary.Add(file, Path.GetFileName(file));                              
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
                        // Enable deletion of read only files
                        File.SetAttributes(file, FileAttributes.Normal);
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

        // Handles replicating files, and keeping their attributes synchronized. This also handles hidden / read-only files.
        private void HandleFileReplication(string sourceFilePath)
        {
            FileAttributes sourceFileAttributes = File.GetAttributes(sourceFilePath);
            string targetFilePath = GetAbsoluteTargetPath(sourceFilePath);

            try
            {
                File.SetAttributes(sourceFilePath, FileAttributes.Normal);
                if (File.Exists(targetFilePath)) File.SetAttributes(targetFilePath, FileAttributes.Normal);
                File.Copy(sourceFilePath, targetFilePath, true);
                File.SetAttributes(sourceFilePath, sourceFileAttributes);
                File.SetAttributes(targetFilePath, sourceFileAttributes);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File has not been found during replication.");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Access denied in replication.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
                throw;
            }
        }

        // Handles replicating directories, and keeping their attributes synchronized. This also handles hidden / read-only directories.
        private void HandleDirectoryReplication(string sourceDirectoryPath)
        {

            string absoluteTargetPath = GetAbsoluteTargetPath(sourceDirectoryPath);
            FileAttributes sourceFileAttributes = File.GetAttributes(sourceDirectoryPath);

            try
            {
                Directory.CreateDirectory(absoluteTargetPath);
                File.SetAttributes(absoluteTargetPath, sourceFileAttributes);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory has not been found during replication.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied in replication.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error. Please try again.");
                throw;
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
