# SyncTask
 Folder synchronization task for a job application by Filip Stolin. This program synchronizes the source directory to replica directory in a one-way manner and logs changes to the specified log file. The synchronization runs periodically in interval set by the arguments.

 ## Features
  - Single recursive directory traversal for each source and replica directory every sync.
  - MD5 file content change detection.
  - Attribute change detection for files & folders.
  - Event-triggered logging.
  - Detection of file manipulation, missing and extra files in the replica.

 ## Parameters
 1. **source path** - The path to the source directory.
 2. **replica path** - The path to the replica directory.
 3. **log file path** - The path to the log file. The log file shouldn't be located in the replica directory or its subdirectories.
 4. **interval** [s] - Optional. Synchronization interval in seconds, default value is 10 seconds.

 ## Controls
 - Press any key to exit the synchronization process at any time.

 ## Example usage
 Syntax
 ```bash
dotnet run -- <source-path> <replica-path> <log-file-path> [<interval>]
 ```
Run synchronization each 5 seconds
 ```bash
dotnet run -- C:\Sync\Source C:\Sync\Target C:\Sync\Log.txt 5
 ```

## Known limitations
- Every file and folder from the source directory is always replicated and logged during the first synchronization, even if they exist in the replica directory.
