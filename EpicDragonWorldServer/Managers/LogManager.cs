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
            DateTime currentTime;
            StreamWriter writer;
            string fileName;
            int writeCount;

            while (true)
            {
                // Update time needed for file name format.
                currentTime = DateTime.Now;

                // Append to "log\Console yyyy-MM-dd.txt" file.
                writeCount = CONSOLE_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    fileName = GetFileName(LOG_FILE_CONSOLE, currentTime);
                    using (writer = File.AppendText(fileName))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            writer.WriteLine(CONSOLE_LOG_CACHE[i]);
                        }
                    }
                    // Remove from cache.
                    lock (CONSOLE_LOG_CACHE)
                    {
                        for (int i = writeCount - 1; i >= 0; i--)
                        {
                            CONSOLE_LOG_CACHE.RemoveAt(i);
                        }
                    }
                }

                // Append to "log\World yyyy-MM-dd.txt" file.
                writeCount = WORLD_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    fileName = GetFileName(LOG_FILE_WORLD, currentTime);
                    using (writer = File.AppendText(fileName))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            writer.WriteLine(WORLD_LOG_CACHE[i]);
                        }
                    }
                    // Remove from cache.
                    lock (WORLD_LOG_CACHE)
                    {
                        for (int i = writeCount - 1; i >= 0; i--)
                        {
                            WORLD_LOG_CACHE.RemoveAt(i);
                        }
                    }
                }

                // Append to "log\Chat yyyy-MM-dd.txt" file.
                writeCount = CHAT_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    fileName = GetFileName(LOG_FILE_CHAT, currentTime);
                    using (writer = File.AppendText(fileName))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            writer.WriteLine(CHAT_LOG_CACHE[i]);
                        }
                    }
                    // Remove from cache.
                    lock (CHAT_LOG_CACHE)
                    {
                        for (int i = writeCount - 1; i >= 0; i--)
                        {
                            CHAT_LOG_CACHE.RemoveAt(i);
                        }
                    }
                }

                // Append to "log\Admin yyyy-MM-dd.txt" file.
                writeCount = ADMIN_LOG_CACHE.Count;
                if (writeCount > 0)
                {
                    fileName = GetFileName(LOG_FILE_ADMIN, currentTime);
                    using (writer = File.AppendText(fileName))
                    {
                        for (int i = 0; i < writeCount; i++)
                        {
                            writer.WriteLine(ADMIN_LOG_CACHE[i]);
                        }
                    }
                    // Remove from cache.
                    lock (ADMIN_LOG_CACHE)
                    {
                        for (int i = writeCount - 1; i >= 0; i--)
                        {
                            ADMIN_LOG_CACHE.RemoveAt(i);
                        }
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
        DateTime currentTime = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(string.Format(LOG_DATE_FORMAT, currentTime));
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
        // Format message with date.
        DateTime currentTime = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(string.Format(LOG_DATE_FORMAT, currentTime));
        sb.Append("] ");
        sb.Append(message);
        message = sb.ToString();

        // Cache message for write to file task.
        lock (WORLD_LOG_CACHE)
        {
            WORLD_LOG_CACHE.Add(message);
        }
    }

    public static void LogChat(string message)
    {
        // Format message with date.
        DateTime currentTime = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(string.Format(LOG_DATE_FORMAT, currentTime));
        sb.Append("] ");
        sb.Append(message);
        message = sb.ToString();

        // Cache message for write to file task.
        lock (CHAT_LOG_CACHE)
        {
            CHAT_LOG_CACHE.Add(message);
        }
    }

    public static void LogAdmin(string message)
    {
        // Format message with date.
        DateTime currentTime = DateTime.Now;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        sb.Append(string.Format(LOG_DATE_FORMAT, currentTime));
        sb.Append("] ");
        sb.Append(message);
        message = sb.ToString();

        // Cache message for write to file task.
        lock (ADMIN_LOG_CACHE)
        {
            ADMIN_LOG_CACHE.Add(message);
        }
    }
}
