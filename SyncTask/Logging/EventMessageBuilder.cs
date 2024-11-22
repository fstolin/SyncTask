using SyncTask.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Logging
{
    internal class EventMessageBuilder : IEventMessageBuilder
    {

        public string BuildMessage(LogEventArgs _event)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            MessageType messageType = _event.MessageType;
            string message;

            if (_event.ItemType == null)
            {
               return ($"[{time}] [{_event.MessageType}] {_event.Message}");
            }

            switch (messageType)
            {
                case MessageType.Added:
                    message = $"[{time}] [{messageType}] {_event.ItemType} has been added to source directory and copied to replica: {_event.Message}";
                    break;
                case MessageType.Modified:
                    message = $"[{time}] [{messageType}] Modified {_event.ItemType} was updated to match source: {_event.Message}";
                    break;
                case MessageType.Removed:
                    message = $"[{time}] [{messageType}] Removed extra {_event.ItemType} from replica: {_event.Message}.";
                    break;
                case MessageType.Missing:
                    message = $"[{time}] [{messageType}] {_event.ItemType} was missing in replica: {_event.Message}";
                    break;
                case MessageType.Error:
                    message = $"[{time}] [{messageType}] Error with {_event.ItemType}: {_event.Message}";
                    break;
                default:
                    message = $"[{time}] [Unknown] {_event.Message}";
                    break;
            }

            return message;
        }

    }
}
