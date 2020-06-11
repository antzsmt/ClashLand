using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClashLand.Core;
using ClashLand.Extensions;
using System.Threading;
using ClashLand.Core.Networking;
using ClashLand.Extensions.List;
using ClashLand.core.Checker;
using System.Timers;
using System.Drawing;
using ClashLand.Core.Core.API.Discord;

namespace ClashLand
{
    internal class Program
    {
        internal static Stopwatch Stopwatch = Stopwatch.StartNew();
        public static Stopwatch _Stopwatch = new Stopwatch();
        private static int Width = 120;
        private static int Height = 30;

        public static int OP;
        

        internal static void Main()
        {

            NativeCalls.SetWindowLong(NativeCalls.GetConsoleWindow(), -20, (int)NativeCalls.GetWindowLong(NativeCalls.GetConsoleWindow(), -20) ^ 0x80000);
            Console.Title = $"ClashLand Server V{Constants.Version} - Developer - {DateTime.Now.Year} ©";
            Console.SetOut(new Prefixed());
            Console.SetWindowSize(Program.Width, Program.Height);

            ClashLand.Core.Consoles.Colorful.Console.WriteWithGradient(@"

                         _________ .__                .__     .____                       .___
                         \_   ___ \|  | _____    _____|  |__  |    |   _____    ____    __| _/
                         /    \  \/|  | \__  \  /  ___/  |  \ |    |   \__  \  /    \  / __ |
                         \     \___|  |__/ __ \_\___ \|   Y  \|    |___ / __ \|   |  \/ /_/ | 
                          \______  /____(____  /____  >___|  /|_______ (____  /___|  /\____ | 
                                 \/          \/     \/     \/         \/    \/     \/      \/       
        
                                      Version 9.256 Support (+ v10.134 mod)
            ", Color.Yellow, Color.Fuchsia, 14);



            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(@"ClashLand is protected by our policies, available only to our partner.");
            Console.WriteLine(@"ClashLand program is under the 'Proprietary' license.");
            Console.WriteLine(@"ClashLand is NOT affiliated to 'Supercell Oy'.");
            Console.WriteLine(@"-----------------------------------------------------");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + @" is now starting..." + Environment.NewLine);

            Resources.Initialize();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"-------------------------------------" + Environment.NewLine);
           
            Thread.Sleep(Timeout.Infinite);
    
        }

        internal static void TitleDe()
        {
            Console.Title = Constants.DefaultTitle + Interlocked.Decrement(ref OP);
        }
    }
}