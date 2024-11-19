﻿using SyncTask.Logging;
using SyncTask.ArgumentHandling;
using SyncTask.Utils;
using SyncTask.Services;
using System.Timers;
using SyncTask.Exceptions;

namespace SyncTask
{
    internal class Program
    {

        static void Main(string[] args)
        {
            try
            {
                ArgumentHandler argumentHandler = new ArgumentHandler(args);
                SyncRuntimeManager syncRuntimeManager = new SyncRuntimeManager(argumentHandler.GetValidArguments());
            }
            catch (InvalidCmdParametersException)
            {
                Console.WriteLine("Error starting the synchronization.");
                Console.ReadKey();
            }
        }
    }
}
