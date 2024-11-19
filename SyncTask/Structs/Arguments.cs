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
