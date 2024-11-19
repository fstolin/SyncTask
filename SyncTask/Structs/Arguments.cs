using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Structs
{
    public struct Arguments
    {

        public string SourcePath { get; }
        public string TargetPath { get; }
        public string LogFilePath { get; }
        public float Interval { get; }

        public Arguments(string sourcePath, string targetPath, string logFilePath, float interval)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            LogFilePath = logFilePath;
            Interval = interval;
        }
    }
}
