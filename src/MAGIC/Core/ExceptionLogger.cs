using System;
using System.IO;
using System.Reflection;
using ClashLand.Extensions;
using SharpRaven;
using SharpRaven.Data;

namespace ClashLand.Core
{
    internal class ExceptionLogger
    {

        private static readonly object s_sync = new object();
        internal static RavenClient RavenClient;

        internal static void Initialize()
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string Enviroment;
            if (Constants.Local)
                Enviroment = "local";
            else
            {
#if DEBUG
                Enviroment = "debug";
#else
                Enviroment = "production";
#endif
            }
           RavenClient =
                new RavenClient(
                    "https://c011a726734a4b5baf17f2d1a7519374:534d376264934c7084a24079726b8e99@sentry.io/187787")
                {
                    Environment = Enviroment,
                    Release = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    Timeout = TimeSpan.FromSeconds(5)
                };
        }

        public static void Log(Exception ex, string moreInfo, string Model = "", string OS = "", string Token = "", long ID = 0)
        {
            var type = ex.GetType();
            var name = type.Name;
            var dirPath = Path.Combine("logs", name);

            if (!Directory.Exists(name))
                Directory.CreateDirectory(dirPath);

            var text = moreInfo + ": " + ex + "\r\n\r\n";
            var filePath = Path.Combine(dirPath, GetLogFileName(ex));

            lock (s_sync)
                File.AppendAllText(filePath, text);

            //if (Constants.Verbosity > 2)
            //Logger.Error(moreInfo + ": " + ex.GetType().Name + ": " + ex.Message);

            if (Constants.UseSentry)
            {
                SentryEvent Event = new SentryEvent(ex)
                {
                    Message = moreInfo,
                };


                Event.Tags.Add("token", Token);
                Event.Tags.Add("userid", ID.ToString());
                Event.Tags.Add("model", Model);
                Event.Tags.Add("os", OS);

                RavenClient.Capture(Event);
            }
        }
        private static string GetLogFileName(Exception ex)
        {
            if (string.IsNullOrWhiteSpace(ex.Message))
                return "unknown.log";
            else
                return StripString(ex.Message).Replace(".", "") + ".log";
        }

        private static string StripString(string value)
        {
            // Trim the string.
            if (value.Length > 50)
                value = value.Substring(0, 50);

            var invalidChars = Path.GetInvalidFileNameChars();
            var final = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                var invalid = false;
                for (int j = 0; j < invalidChars.Length; j++)
                {
                    if (value[i] == invalidChars[j])
                    {
                        invalid = true;
                        break;
                    }
                }

                if (!invalid)
                    final += value[i];
            }
            return final;
        }
    }
}
