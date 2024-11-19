using System.Text;
using System.Security.Cryptography;
using SyncTask.Logging;

namespace SyncTask.Utilities
{
    static class HashUtils
    {

        public static event EventHandler<LogEventArgs>? UtilsLogMessageSent;
        private static readonly MD5 md5 = MD5.Create();

        // Returns a hash for file content & metadata, or only file content with simple argument.
        public static string? GetFileHash(string path, bool simple = false)
        {
            try
            {
                // File content hash
                FileStream fileStream = File.OpenRead(path);
                byte[] hashBytes = md5.ComputeHash(fileStream);
                fileStream.Close();
                string fileHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                if (!simple)
                {
                    // Metadata hash
                    // Get file metadata
                    FileInfo fileInfo = new FileInfo(path);
                    byte[] metadataBytes = Encoding.UTF8.GetBytes(
                        fileInfo.CreationTimeUtc.ToString("o") +
                        fileInfo.LastWriteTimeUtc.ToString("o") +
                        fileInfo.Attributes.ToString()
                    );
                    byte[] metadataHashBytes = md5.ComputeHash(metadataBytes);
                    string metaHash = BitConverter.ToString(metadataHashBytes).Replace("-", "").ToLower();
                    return ($"{fileHash}_{metaHash}");
                } else
                {
                    return fileHash;
                }

                
            }
            catch (FileNotFoundException)
            {
                UtilsLogMessageSent?.Invoke(null, new LogEventArgs($"File {path} not found", MessageType.Error));
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                UtilsLogMessageSent?.Invoke(null, new LogEventArgs($"Directory {path} not found", MessageType.Error));
                return null;
            }
            catch (IOException)
            {
                UtilsLogMessageSent?.Invoke(null, new LogEventArgs($"IO exception for: {path}", MessageType.Error));
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                UtilsLogMessageSent?.Invoke(null, new LogEventArgs($"Access denied for: {path}", MessageType.Error));
                return null;
            }
            catch (Exception e)
            {
                UtilsLogMessageSent?.Invoke(null, new LogEventArgs(e.Message, MessageType.Error));
                return null;
            }
        }

        public static bool AreHashesEqual(string? hash1, string? hash2)
        {
            if (hash1 == null || hash2 == null) return false;
            return hash1.Equals(hash2, StringComparison.Ordinal);
        }

    }
}
