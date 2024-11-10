using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Logging
{
    public enum MessageType
    {
        Changed,
        Added,
        Removed,
        Error
    }

    public enum ItemType
    {
        file,
        folder
    }

    public class LogEventArgs : EventArgs
    {
        public string Path { get; set; }
        public MessageType MessageType { get; set; }
        public ItemType ItemType { get; set; }

        public LogEventArgs(string message, MessageType messageType, ItemType itemType) 
        { 
            Path = message; 
            MessageType = messageType;
            ItemType = itemType;
        }
    }
}
