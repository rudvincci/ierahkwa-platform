using System.Diagnostics;
using System.IO;
using Mamey.Types;
using System.Net.NetworkInformation;

namespace Mamey;

public static class DiagnosticsExtensions
{
    // Helper Functions
    // Get system uptime
    private static string GetUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }

    // Get CPU usage
    private static string GetCpuUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount:F2}%";
    }

    // Get memory usage
    public static string GetMemoryUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.WorkingSet64 / (1024 * 1024)} MB used";
    }

    // Get total memory used by the application
    private static string GetTotalMemory()
    {
        return $"{GC.GetTotalMemory(false) / (1024 * 1024)} MB";
    }

    // Get available disk space
    private static string GetDiskSpace()
    {
        var driveInfo = new DriveInfo(Directory.GetCurrentDirectory());
        var availableSpace = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024); // GB
        var totalSpace = driveInfo.TotalSize / (1024 * 1024 * 1024); // GB
        return $"{availableSpace} GB free of {totalSpace} GB";
    }

    // Get the number of threads currently being used by the application
    private static int GetThreadCount()
    {
        return Process.GetCurrentProcess().Threads.Count;
    }

    // Get the total number of garbage collections performed (per generation)
    private static string GetGarbageCollectionCount()
    {
        return $"Gen0: {GC.CollectionCount(0)}, Gen1: {GC.CollectionCount(1)}, Gen2: {GC.CollectionCount(2)}";
    }

    // Get network usage (data sent/received over network interfaces)
    private static string GetNetworkUsage()
    {
        long totalSent = 0;
        long totalReceived = 0;

        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            var stats = ni.GetIPv4Statistics();
            totalSent += stats.BytesSent;
            totalReceived += stats.BytesReceived;
        }

        return $"Sent: {totalSent / (1024 * 1024)} MB, Received: {totalReceived / (1024 * 1024)} MB";
    }

    public static ServerStatus GetServerStatus(AppOptions options)
    {
        return new ServerStatus
        {
            Uptime = GetUptime(),
            CpuUsage = GetCpuUsage(),
            MemoryUsage = GetMemoryUsage(),
            TotalMemory = GetTotalMemory(),
            DiskSpace = GetDiskSpace(),
            ThreadCount = GetThreadCount(),
            GcCollections = GetGarbageCollectionCount(),
            NetworkUsage = GetNetworkUsage(),
            Timestamp = DateTime.UtcNow,
            AppName = options.Name,
            AppVersion = options.Version,
            MachineName = Environment.MachineName
        };
    }

    public class ServerStatus
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string MachineName { get; set; }
        public string Uptime { get; set; }
        public string CpuUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string TotalMemory { get; set; }
        public string DiskSpace { get; set; }
        public int ThreadCount { get; set; }
        public string GcCollections { get; set; }
        public string NetworkUsage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}