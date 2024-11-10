using SyncTask.Logging;
using SyncTask.Utils;
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
        public event EventHandler<LogEventArgs>? LogMessageSent;

        private readonly string initialSourcePah;
        private readonly string targetPath;
        // Keeps the path as key, MD5 has as value, used to compare files
        private Dictionary<string, string?> sourceFilesDictionary;
        // Set to keep track of currently active file / directories. Used by
        // CleanUp / deletion of extra replica files.
        private HashSet<string> currentSourceFiles;

        public ReplicationManager(string sourcePath, string targetPath) 
        {
            initialSourcePah = sourcePath;
            this.targetPath = targetPath;
            sourceFilesDictionary = new Dictionary<string, string?>();
            currentSourceFiles = new HashSet<string>();
        }

        // Initializes the replication process
        public void InitializeReplication()
        {
            try
            {
                currentSourceFiles.Clear();
                Directory.CreateDirectory(targetPath);
                ReplicateDirectory(initialSourcePah);
                CleanUpReplica(targetPath);
                CleanUpSourceDictionary();
                LogMessageSent?.Invoke(this, new LogEventArgs("Synchronization finished.", MessageType.Info));
            }
            catch(IOException) {
                RaiseErrorMessage("I/O error. Please try again.");
            }
            catch (NotSupportedException)
            {
                RaiseErrorMessage("Invalid target directory path. Please try again.");
            }
            catch (UnauthorizedAccessException e)
            {
                RaiseErrorMessage("Access denied in initialization.");
            }
            catch (Exception e)
            {
                RaiseErrorMessage($"Unexpected error. Please try again. + {e.Message}");
                throw;
            }
            
        }

        // Recursively goes through files and subfolders in selectedPath.
        private void ReplicateDirectory(string sourcePath)
        {
            string[] subFolders = Directory.GetDirectories(sourcePath);
            string[] files = Directory.GetFiles(sourcePath);

            // Traversing all folders
            foreach (string subFolder in subFolders)
            {
                ProcessDirectory(subFolder);                
                currentSourceFiles.Add(subFolder);

                // Traverse next subfolder
                ReplicateDirectory(subFolder);
            }

            // Traversing all files
            foreach (string file in files)
            {
                ProcessFile(file);
            }
        }

        // Processes file, and decides, whether it will be copied and on what grounds
        private void ProcessFile(string file) 
        {
            string? hash = HashUtils.GetFileHash(file);
            currentSourceFiles.Add(file);

            // Added a new file - if it wasn't in dictionary
            if (!sourceFilesDictionary.ContainsKey(file))
            {
                sourceFilesDictionary.Add(file, hash);
                HandleFileReplication(file, MessageType.Added);
            }
            // Update a modified file (content / metadata)
            else if (hash != null && !HashUtils.AreHashesEqual(hash, sourceFilesDictionary[file]))
            {
                sourceFilesDictionary[file] = hash;
                HandleFileReplication(file, MessageType.Modified);
            }
            // Copy a file missing in replica. We know that the file is
            // in the dictionary due to the first condition
            else if (IsMissingInReplica(file))
            {
                sourceFilesDictionary[file] = hash;
                HandleFileReplication(file, MessageType.Missing);
            }
            // We now know, the file is present in the replica
            // Update a file when attributes are changed only in the replica
            else if (!AreAttributesEqual(file))
            {
                sourceFilesDictionary[file] = hash;
                HandleFileReplication(file, MessageType.Modified);
            }
            // Update a file when the content is changed only in the replica
            else if (!IsTargetHashEqual(file))
            {
                sourceFilesDictionary[file] = hash;
                HandleFileReplication(file, MessageType.Modified);
            }
        }

        // Processes a directory, and decides, whether it will be copied and on what grounds
        private void ProcessDirectory(string directory)
        {
            // Add missing folders to replica
            if (IsDirectoryMissingInReplica(directory))
            {
                HandleFolderReplication(directory);
                RaiseItemChanged(directory, MessageType.Added, ItemType.folder);
            }
            // Modify attributes in replica according to source
            else if (!AreAttributesEqual(directory))
            {
                HandleFolderReplication(directory);
                RaiseItemChanged(directory, MessageType.Modified, ItemType.folder);
            }
        }

        // Copies files, and keeps their attributes synchronized. This also handles hidden / read-only files.
        private void HandleFileReplication(string sourceFilePath, MessageType type)
        {
            FileAttributes sourceFileAttributes = File.GetAttributes(sourceFilePath);
            string targetFilePath = GetAbsoluteTargetPath(sourceFilePath);

            try
            {
                // Copy the file
                File.SetAttributes(sourceFilePath, FileAttributes.Normal);
                if (File.Exists(targetFilePath)) File.SetAttributes(targetFilePath, FileAttributes.Normal);
                File.Copy(sourceFilePath, targetFilePath, true);
                File.SetAttributes(sourceFilePath, sourceFileAttributes);
                File.SetAttributes(targetFilePath, sourceFileAttributes);
                // Raise item changed event
                RaiseItemChanged(sourceFilePath, type, ItemType.file);
            }
            catch (FileNotFoundException)
            {
                RaiseErrorMessage("File has not been found during replication.");
            }
            catch (UnauthorizedAccessException e)
            {
                RaiseErrorMessage("Access denied in replication.");
            }
            catch (Exception e)
            {
                RaiseErrorMessage($"Unexpected error. Please try again. + {e.Message}");
                throw;
            }
        }

        // Copies directories, and keeps their attributes synchronized. This also handles hidden / read-only directories.
        private void HandleFolderReplication(string sourceDirectoryPath)
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
                RaiseErrorMessage("Directory has not been found during replication.");
            }
            catch (UnauthorizedAccessException)
            {
                RaiseErrorMessage("Access denied in replication.");
            }
            catch (Exception)
            {
                RaiseErrorMessage("Unexpected error. Please try again.");
                throw;
            }

        }

        // Deletes files in replica, which are not in the source directory (dictionary).
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

                    }
                    else
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
                RaiseErrorMessage("The source directory doesn't exist during cleanup. Please try again.");
            }
            catch (FileNotFoundException)
            {
                RaiseErrorMessage("File has not been found during cleanup.");
            }
            catch (UnauthorizedAccessException)
            {
                RaiseErrorMessage("Access denied during cleanup. Please try again.");
            }
            catch (Exception e)
            {
                RaiseErrorMessage($"Unexpected error. Please try again. + {e.Message}");
            }
        }

        // At the end of synchronization: Removes removed files in source directory from the dictionary.
        private void CleanUpSourceDictionary()
        {
            foreach (string item in sourceFilesDictionary.Keys)
            {
                if (!currentSourceFiles.Contains(item))
                {
                    sourceFilesDictionary.Remove(item);
                }
            }
        }

        // Returns true if the file is missing from source dictionary, meaning it should be deleted
        private bool ShouldBeRemoved(string targetAbsolutePath)
        {
            if (currentSourceFiles.Contains(GetAbsoluteSourcePath(targetAbsolutePath)))
            {
                return false;
            }
            return true;
        }

        // Compares source file attributes with target file attributes
        private bool AreAttributesEqual(string sourceAbsolutePath)
        {
            try
            {
                string targetAbsolutePath = GetAbsoluteTargetPath(sourceAbsolutePath);
                FileAttributes sourceAttributes = File.GetAttributes(sourceAbsolutePath);
                FileAttributes targetAttributes = File.GetAttributes(targetAbsolutePath);
                return sourceAttributes.Equals(targetAttributes);
            }
            catch (FileNotFoundException)
            {
                RaiseErrorMessage("File has not been found during attribute comparison.");
                return false;
            }
            catch (IOException)
            {
                RaiseErrorMessage("IO Exception during attribute comparison.");
                return false;

            }
            catch (Exception e)
            {
                RaiseErrorMessage($"Unexpected error when comparing Attributes. Please try again. + {e.Message}");
                return false;
            }
        }

        // Compares a hash to a generated hash of the file in the replica (simple hash only - no metada)
        private bool IsTargetHashEqual(string sourcePath)
        {
            string targetAbsolutePath = GetAbsoluteTargetPath(sourcePath);
            string? targetHash = HashUtils.GetFileHash(targetAbsolutePath, true);
            string? hash = HashUtils.GetFileHash(sourcePath, true);
            if (targetHash == null || hash == null)
            {
                return false;
            }
            if (HashUtils.AreHashesEqual(hash, targetHash))
            {
                return true;
            } else
            {
                return false;
            }
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
            LogMessageSent?.Invoke(this, new LogEventArgs(path, messageType, itemType));
        }

        private void RaiseErrorMessage(string message)
        {
            LogMessageSent?.Invoke(this, new LogEventArgs(message, MessageType.Error));
        }
        private bool IsMissingInReplica(string sourceAbsolutePath)
        {
            string targetAbsolutePath = GetAbsoluteTargetPath(sourceAbsolutePath);
            return !File.Exists(targetAbsolutePath);
        }

        private bool IsDirectoryMissingInReplica(string sourceAbsolutePath)
        {
            string targetAbsolutePath = GetAbsoluteTargetPath(sourceAbsolutePath);
            return !Directory.Exists(targetAbsolutePath);
        }
    }
}
