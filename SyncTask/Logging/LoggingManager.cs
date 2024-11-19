using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Logging
{
    class LoggingManager
    {

        private readonly string logFilePath;
        private readonly StreamWriter? streamWriter;

        public LoggingManager(string logFilePath)
        {
            this.logFilePath = logFilePath;

            try
            {
                // Create the directory
                string? directoryPath = Path.GetDirectoryName(logFilePath);
                if (directoryPath == null) return;
                Directory.CreateDirectory(directoryPath);

                // Create or open the file
                FileStream fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                streamWriter = new StreamWriter(fileStream);
            }
            catch (IOException)
            {
                Console.WriteLine("IO exception while instancing streamWriter");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while instancing streamWriter: {e.Message}");
            }
        }

        // Decides whether to log regular message or a message concerning a file / directory change.
        public void OnLogEventRaised(object? sender, LogEventArgs logArgs)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            if (logArgs.ItemType == null)
            {
                LogGeneralMessage(logArgs, time);
            }
            else
            {
                LogFileChanged(logArgs, time);
            }

        }

        // Decide on the logging message from the event args. Log in console / call for logging in file
        private void LogFileChanged(LogEventArgs logArgs, string time)
        {
            MessageType messageType = logArgs.MessageType;
            string message;

            switch (messageType)
            {
                case MessageType.Added:
                    message = $"[{time}] [{messageType}] {logArgs.ItemType} has been added to source directory and copied to replica: {logArgs.Message}";
                    break;
                case MessageType.Modified:
                    message = $"[{time}] [{messageType}] Modified {logArgs.ItemType} was updated to match source: {logArgs.Message}";
                    break;
                case MessageType.Removed:
                    message = $"[{time}] [{messageType}] Removed extra {logArgs.ItemType} from replica: {logArgs.Message}.";
                    break;
                case MessageType.Missing:
                    message = $"[{time}] [{messageType}] {logArgs.ItemType} was missing in replica: {logArgs.Message}";
                    break;
                case MessageType.Error:
                    message = $"[{time}] [{messageType}] Error with {logArgs.ItemType}: {logArgs.Message}";
                    break;
                default:
                    message = $"[{time}] [Unknown] {logArgs.Message}";
                    break;
            }

            Console.WriteLine(message);
            LogToFile(message);
        }

        // Log a message into log file
        private void LogToFile(string message)
        {
            if (streamWriter != null)
            {
                streamWriter.WriteLine(message);
                streamWriter.Flush();
            }
            else
            {
                Console.WriteLine("Error writing to log file. Log file is null.");
            }
        }

        // Logging of general messages
        private void LogGeneralMessage(LogEventArgs logArgs, string time)
        {
            Console.WriteLine($"[{time}] [{logArgs.MessageType}] {logArgs.Message}");
        }
    }
}