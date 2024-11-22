using SyncTask.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Interfaces
{
    public interface ILoggingManager
    {
        public void OnLogEventRaised(object? sender, LogEventArgs logArgs);

    }
}
