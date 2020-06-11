using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClashLand.Core
{
    internal static class Logger
    {
        private static bool _validLogLevel;
        private static readonly int LogLevel = ToInt32(ConfigurationManager.AppSettings["LogLevel"]);

        private static int ToInt32(string valute)
        {
            throw new NotImplementedException();
        }

        private static readonly string Timestamp = Convert.ToString(DateTime.Today).Remove(10).Replace(".", "-").Replace("/", "-");
        private static readonly string Path = "Logs/log_" + Timestamp + "_.txt";
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1);

        private static readonly object s_lock = new object();
        private static readonly string s_errPath = "Logs/err_" + DateTime.Now.ToFileTime() + "_.log";
        private static readonly StreamWriter s_errWriter = new StreamWriter(s_errPath);

        public static void Initialize()
        {
            if (LogLevel > 2)
            {
                _validLogLevel = false;
                LogLevelError();
            }
            else
            {
                _validLogLevel = true;
            }

            if (LogLevel != 0 || _validLogLevel == true)
            {
                if (!File.Exists("Logs/log_" + Timestamp + "_.txt"))
                {
                    using (var sw = new StreamWriter("Logs/log_" + Timestamp + "_.txt"))
                    {
                        sw.WriteLine("Log file created at " + DateTime.Now);
                        sw.WriteLine();
                    }
                }
            }
        }

        public static async void Write(string text)
        {
            if (LogLevel != 0)
            {
                try
                {
                    await _fileLock.WaitAsync();
                    if (LogLevel == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[LOG]    " + text);
                        Console.ResetColor();
                    }

                    using (StreamWriter sw = new StreamWriter(Path, true))
                        await sw.WriteLineAsync("[LOG]    " + text + " at " + DateTime.UtcNow);
                }
                finally
                {
                    _fileLock.Release();
                }
            }
        }

        public static void SayInfo(string message)
        {
            lock (s_lock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
                Console.WriteLine(message);
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                Console.ResetColor();
            }
        }

        public static void SayAscii(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (message.Length <= Console.WindowWidth)
            {
                Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
            }

            Console.WriteLine(message);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }

        public static void Say(string message, ConsoleColor color = ConsoleColor.Green)
        {
            lock (s_lock)
            {
                Console.ForegroundColor = color;
                if (message.Length <= Console.WindowWidth)
                {
                    Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
                }
                Console.WriteLine(message);
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                Console.ResetColor();
            }
        }

        public static void Say()
        {
            lock (s_lock)
            {
                Console.WriteLine();
            }
        }

        public static void Error(string message)
        {
            lock (s_lock)
            {
                var text = "[ERROR]  " + message;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ResetColor();

                s_errWriter.WriteLine(text);
            }
        }

        private static void LogLevelError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Please choose a valid Log Level");
            Console.WriteLine("Emulator is now closing...");
            Console.ResetColor();
            Thread.Sleep(5000);
            Environment.Exit(0);
        }
    }
}

