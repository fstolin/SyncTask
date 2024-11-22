using SyncTask.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Logging
{
    internal class ConsoleLoggingManager : ILoggingManager
    {
        private readonly IEventMessageBuilder _messageBuilder;

        public ConsoleLoggingManager(IEventMessageBuilder messageBuilder)
        {
            _messageBuilder = messageBuilder;
        }

        public void OnLogEventRaised(object? sender, LogEventArgs logArgs)
        {
            Console.WriteLine(_messageBuilder.BuildMessage(logArgs));
        }
    }
}
