using SyncTask.Logging;
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
        public event EventHandler<LogEventArgs>? ItemChanged;

        private readonly string initialSourcePah;
        private readonly string targetPath;
        private Dictionary<string, string> sourceFilesDictionary;

        public ReplicationManager(string sourcePath, string targetPath) 
        {
            initialSourcePah = sourcePath;
            this.targetPath = targetPath;
            sourceFilesDictionary = new Dictionary<string, string>();
        }

        // Initializes the replication process
        public void InitializeReplication()
        {
            try
            {
                // Temporary hack
                sourceFilesDictionary.Clear();
                Directory.CreateDirectory(targetPath);
                ReplicateDirectory(initialSourcePah);
                CleanUpReplica(targetPath);
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
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error. Please try again. + {e.Message}");
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
                sourceFilesDictionary.Add(subFolder, Path.GetFileName(subFolder));
                // Traverse next subfolder
                ReplicateDirectory(subFolder);
            }

            // Traversing all files
            foreach (string file in files)
            {
                HandleFileReplication(file);
                sourceFilesDictionary.Add(file, Path.GetFileName(file));                              
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
                    if (ShouldBeRemoved(subFolder))
                    {
                        Directory.Delete(subFolder, true);
                        RaiseItemChanged(subFolder, MessageType.Removed, ItemType.folder);
                        
                    } else
                    {
                        CleanUpReplica(subFolder);
                    }
                }

                 //Traversing all files
                foreach (string file in files)
                {
                    if (ShouldBeRemoved(file))
                    {
                        // Enable deletion of read only files
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                        RaiseItemChanged(file, MessageType.Removed, ItemType.file);
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
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error. Please try again. + {e.Message}");
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
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error. Please try again. + {e.Message}");
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
        private bool ShouldBeRemoved(string targetAbsolutePath)
        {
            if (sourceFilesDictionary.ContainsKey(GetAbsoluteSourcePath(targetAbsolutePath)))
            {
                return false;
            }
            return true;
        }

        // Converts from original absolute path to absolute path for the target directory
        private string GetAbsoluteTargetPath(string path)
        {
            string relativePath = Path.GetRelativePath(initialSourcePah, path);
            return Path.Combine(targetPath, relativePath);
        }

        // Converts from target absolute path to original absolute path
        private string GetAbsoluteSourcePath(string path)
        {
            string relativePath = Path.GetRelativePath(targetPath, path);
            return Path.Combine (initialSourcePah, relativePath);
        }

        // Invokes OnFileChanged event for a directory / file
        private void RaiseItemChanged(string path, MessageType messageType, ItemType itemType)
        {
            ItemChanged?.Invoke(this, new LogEventArgs(path, messageType, itemType));
        }
    }
}
