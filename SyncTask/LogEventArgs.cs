using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Logging
{
    public enum MessageType
    {
        Modified,
        Added,
        Removed,
        Missing,
        Error,
        Info
    }

    public enum ItemType
    {
        file,
        folder
    }

    // Event arguments for logging purposes
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
        public ItemType? ItemType { get; set; }

        public LogEventArgs(string message, MessageType messageType, ItemType? itemType = null) 
        { 
            Message = message; 
            MessageType = messageType;
            ItemType = itemType;
        }
    }
}
