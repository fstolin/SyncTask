using SyncTask.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask
{
    class LoggingManager
    {
        
        private readonly string LogFilePath;

        public LoggingManager(string logFilePath)
        {
            LogFilePath = logFilePath;
        }

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

        // Logging of file (item) changed info
        private void LogFileChanged(LogEventArgs logArgs, string time)
        {
            MessageType messageType = logArgs.MessageType;
            switch (messageType)
            {
                case MessageType.Added:
                    Console.WriteLine($"[{time}] [{messageType}] {logArgs.ItemType} has been added to source directory and copied to replica: {logArgs.Message}");
                    break;
                case MessageType.Modified:
                    Console.WriteLine($"[{time}] [{messageType}] Modified {logArgs.ItemType} was updated to match source: {logArgs.Message}");
                    break;
                case MessageType.Removed:
                    Console.WriteLine($"[{time}] [{messageType}] Removed extra {logArgs.ItemType} from replica: {logArgs.Message}.");
                    break;
                case MessageType.Missing:
                    Console.WriteLine($"[{time}] [{messageType}] {logArgs.ItemType} was missing in replica: {logArgs.Message}");
                    break;
                case MessageType.Error:
                    Console.WriteLine($"[{time}] [{messageType}] Error with {logArgs.ItemType}: {logArgs.Message}");
                    break;
            }
        }

        // Logging of general messages
        private void LogGeneralMessage(LogEventArgs logArgs, string time)
        {
            Console.WriteLine($"[{time}] [{logArgs.MessageType}] {logArgs.Message}");
        }
    }
}