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

        public void OnFileChangedEvent(object? sender, LogEventArgs logArgs)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            switch (logArgs.MessageType)
            {
                case MessageType.Added:
                    Console.WriteLine($"[{time}] {logArgs.ItemType} has been added to source directory: {logArgs.Path}");
                    break;
                case MessageType.Changed:

                    break;
                case MessageType.Removed:
                    Console.WriteLine($"[{time}] Removed {logArgs.ItemType} from replica: {logArgs.Path} - {logArgs.ItemType} is not present in the source directory.");
                    break;
                case MessageType.Error:
                    Console.WriteLine("Error!");
                    break;
            }
            
        }

    }
}
