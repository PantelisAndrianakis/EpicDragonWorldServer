using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class LogManager
{
    public static readonly string LOG_DATE_FORMAT = "{0:dd/MM HH:mm:ss}";

    private static readonly string LOG_FILE_NAME_FORMAT = "{0:yyyy-MM-dd}";
    private static readonly string LOG_PATH = "Log" + Path.DirectorySeparatorChar;
    private static readonly string LOG_FILE_CONSOLE = "Console ";
    private static readonly string LOG_FILE_WORLD = "World ";
    private static readonly string LOG_FILE_CHAT = "Chat ";
    private static readonly string LOG_FILE_ADMIN = "Admin ";
    private static readonly string LOG_FILE_EXT = ".txt";
    private static readonly List<string> CONSOLE_LOG_CACHE = new List<string>();
    private static readonly List<string> WORLD_LOG_CACHE = new List<string>();
    private static readonly List<string> CHAT_LOG_CACHE = new List<string>();
    private static readonly List<string> ADMIN_LOG_CACHE = new List<string>();
    private static readonly List<DateTime> WORLD_DATE_CACHE = new List<DateTime>();
    private static readonly List<DateTime> CHAT_DATE_CACHE = new List<DateTime>();
    private static readonly List<DateTime> ADMIN_DATE_CACHE = new List<DateTime>();
    private static readonly int WRITE_TASK_DELAY = 1000;

    public static void Init()
    {
        // Create Log directory used by LogManager.
        Directory.CreateDirectory("Log");

        // Start the write to disk task.
        new LogManager().WriteToDisk().Wait();
    }

    private async Task WriteToDisk()
    {
        // Repeating task for writing logs to disk.
        await Task.Run(async () =>
        {
            StringBuilder sb = new StringBuilder();
            DateTime currentTime;
            StreamWriter writer;
            int writeCount;

            while (true)
            {
                // Update time needed for file name format.
                currentTime = DateTime.Now;

                // Append to "log\Console yyyy-MM-dd.txt" file.
                writeCount = CONSOLE_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    using (writer = File.AppendText(GetFileName(LOG_FILE_CONSOLE, currentTime)))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            writer.WriteLine(CONSOLE_LOG_CACHE[i]);
                        }
                    }
                    // Remove from cache.
                    lock (CONSOLE_LOG_CACHE)
                    {
                        CONSOLE_LOG_CACHE.RemoveRange(0, writeCount);
                    }
                }

                // Append to "log\World yyyy-MM-dd.txt" file.
                writeCount = WORLD_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    using (writer = File.AppendText(GetFileName(LOG_FILE_WORLD, currentTime)))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            sb.Clear();
                            sb.Append("[");
                            sb.Append(string.Format(LOG_DATE_FORMAT, WORLD_DATE_CACHE[i]));
                            sb.Append("] ");
                            sb.Append(WORLD_LOG_CACHE[i]);
                            writer.WriteLine(sb.ToString());
                        }
                    }
                    // Remove from cache.
                    lock (WORLD_LOG_CACHE)
                    {
                        WORLD_LOG_CACHE.RemoveRange(0, writeCount);
                        WORLD_DATE_CACHE.RemoveRange(0, writeCount);
                    }
                }

                // Append to "log\Chat yyyy-MM-dd.txt" file.
                writeCount = CHAT_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    using (writer = File.AppendText(GetFileName(LOG_FILE_CHAT, currentTime)))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            sb.Clear();
                            sb.Append("[");
                            sb.Append(string.Format(LOG_DATE_FORMAT, CHAT_DATE_CACHE[i]));
                            sb.Append("] ");
                            sb.Append(CHAT_LOG_CACHE[i]);
                            writer.WriteLine(sb.ToString());
                        }
                    }
                    // Remove from cache.
                    lock (CHAT_LOG_CACHE)
                    {
                        CHAT_LOG_CACHE.RemoveRange(0, writeCount);
                        CHAT_DATE_CACHE.RemoveRange(0, writeCount);
                    }
                }

                // Append to "log\Admin yyyy-MM-dd.txt" file.
                writeCount = ADMIN_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    using (writer = File.AppendText(GetFileName(LOG_FILE_ADMIN, currentTime)))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            sb.Clear();
                            sb.Append("[");
                            sb.Append(string.Format(LOG_DATE_FORMAT, ADMIN_DATE_CACHE[i]));
                            sb.Append("] ");
                            sb.Append(ADMIN_LOG_CACHE[i]);
                            writer.WriteLine(sb.ToString());
                        }
                    }
                    // Remove from cache.
                    lock (ADMIN_LOG_CACHE)
                    {
                        ADMIN_LOG_CACHE.RemoveRange(0, writeCount);
                        ADMIN_DATE_CACHE.RemoveRange(0, writeCount);
                    }
                }

                // Delay.
                await Task.Delay(WRITE_TASK_DELAY);
            }
        });
    }

    private static string GetFileName(string prefix, DateTime currentTime)
    {
        string date = string.Format(LOG_FILE_NAME_FORMAT, currentTime);
        string fileName = LOG_PATH + prefix + date + LOG_FILE_EXT;
        if (Config.LOG_FILE_SIZE_LIMIT_ENABLED && File.Exists(fileName))
        {
            int counter = 1;
            long fileSize = new FileInfo(fileName).Length;
            while (fileSize >= Config.LOG_FILE_SIZE_LIMIT)
            {
                fileName = LOG_PATH + prefix + date + "-" + counter++ + LOG_FILE_EXT;
                fileSize = File.Exists(fileName) ? new FileInfo(fileName).Length : 0;
            }
        }
        return fileName;
    }

    public static void Log(string message)
    {
        // Format message with date.
        DateTime date = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(string.Format(LOG_DATE_FORMAT, date));
        sb.Append("] ");
        sb.Append(message);
        message = sb.ToString();

        // Write to console.
        Console.WriteLine(message);

        // Cache message for write to file task.
        lock (CONSOLE_LOG_CACHE)
        {
            CONSOLE_LOG_CACHE.Add(message);
        }
    }

    public static void LogWorld(string message)
    {
        // Keep current date.
        DateTime date = DateTime.Now;

        // Cache date with message for write to file task.
        lock (WORLD_LOG_CACHE)
        {
            WORLD_DATE_CACHE.Add(date);
            WORLD_LOG_CACHE.Add(message);
        }
    }

    public static void LogChat(string message)
    {
        // Keep current date.
        DateTime date = DateTime.Now;

        // Cache date with message for write to file task.
        lock (CHAT_LOG_CACHE)
        {
            CHAT_DATE_CACHE.Add(date);
            CHAT_LOG_CACHE.Add(message);
        }
    }

    public static void LogAdmin(string message)
    {
        // Keep current date.
        DateTime date = DateTime.Now;

        // Cache date with message for write to file task.
        lock (ADMIN_LOG_CACHE)
        {
            ADMIN_DATE_CACHE.Add(date);
            ADMIN_LOG_CACHE.Add(message);
        }
    }
}
