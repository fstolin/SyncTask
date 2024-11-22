using SyncTask.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Interfaces
{
    // Unnecessary interface -> wanted to have MessageBuilder interface but couldn't implement EventMessage builder when I wanted to Build  a message from the current event
    // E.g. BuildMessage() in this interface
    // Somehow implement event in the implementation, but always change the eventArgs..
    public interface IEventMessageBuilder
    {
        public string BuildMessage(LogEventArgs logArgs);
    }
}
