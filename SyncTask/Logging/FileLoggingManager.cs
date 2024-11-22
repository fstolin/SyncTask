using SyncTask.Interfaces;
using System.IO;

namespace SyncTask.Logging
{
    class FileLoggingManager : ILoggingManager
    {

        private readonly IEventMessageBuilder _messageBuilder;
        private readonly string _logFilePath;
        private StreamWriter? _streamWriter;

        private StreamWriter StreamWriter {
            get {
                if (_streamWriter == null)
                {
                    _streamWriter = PrepareNewStreamWriter();
                }
                return _streamWriter;
            }
        }

        public FileLoggingManager(string logFilePath, IEventMessageBuilder messageBuilder)
        {
            _logFilePath = logFilePath;
            _messageBuilder = messageBuilder;
        }

        // Decides whether to log regular message or a message concerning a file / directory change.
        public void OnLogEventRaised(object? sender, LogEventArgs logArgs)
        {
            LogToFile(_messageBuilder.BuildMessage(logArgs));
        }


        // Log a message into log file
        private void LogToFile(string message)
        {
            StreamWriter.WriteLine(message);
            StreamWriter.Flush();
        }

        private StreamWriter PrepareNewStreamWriter()
        {
            try
            {
                // Create the directory
                string? directoryPath = Path.GetDirectoryName(_logFilePath);
                if (directoryPath == null) throw new ArgumentNullException(nameof(directoryPath));
                Directory.CreateDirectory(directoryPath);
                // Create or access the file
                FileStream fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                return new StreamWriter(fileStream);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Directory not found for logpath in FileLoggingManager.");
                throw;
            }
            catch (IOException)
            {
                Console.WriteLine($"IO exception while instancing streamWriter");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while instancing streamWriter: {e.Message}");
                throw;
            }
        }
    }
}