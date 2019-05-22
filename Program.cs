using System;
using System.Numerics;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using NanoSharp.TaskManagment;
using NanoSharp.TaskManagment.Comands;
using NanoSharp.TaskManagment.Templates;

namespace NanoSharp
{
    class Program
    {
        static void PrintLogo() {
            string[] logo = new [] {
                @" _   _                   _____ _                      ",
                @"| \ | |                 /  ___| |                     ",
                @"|  \| | __ _ _ __   ___ \ `--.| |__   __ _ _ __ _ __  ",
                @"| . ` |/ _` | '_ \ / _ \ `--. \ '_ \ / _` | '__| '_ \ ",
                @"| |\  | (_| | | | | (_) /\__/ / | | | (_| | |  | |_) |",
                @"\_| \_/\__,_|_| |_|\___/\____/|_| |_|\__,_|_|  | .__/ ",
                @"                                               | |    ",
                @"                                               |_|    "
            };
            
            //Console.ForegroundColor = ConsoleColor.Yellow;
            foreach(var line in logo) {
                Console.WriteLine("  " + line);
            }
            Console.WriteLine("  ver. 0.1 (alpha)");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("(c) Sokolov Denis 2019 (MIT-license)");
            Console.WriteLine("Program and library for simulating nanosystems");
            Console.WriteLine();

        }
        static void Main(string[] args)
        {        
            try { 
               PrintLogo();
               Console.WriteLine(Directory.GetCurrentDirectory());

               ComandManager commanager = new ComandManager();
               ITemplate monteCarlo = new MonteCarlo();

               IComand run = new Run();
               (run as Run).Add(monteCarlo);

               IComand cnew = new New();
               (cnew as New).Add(monteCarlo);

               commanager.Add(cnew);
               commanager.Add(run);
            
               if(args.Length == 0) {
                  commanager.Help();
               }
               else {
                 commanager.Start(args);
               }
            }
            catch(Exception exc) {
                Console.WriteLine(exc);
                Console.WriteLine("Exception was saved at 'log_exception.txt' file");
                StreamWriter sw = new StreamWriter("log_exception.txt", true, Encoding.Unicode);
                sw.WriteLine(exc);
                sw.Close(); sw.Dispose();
            }
        }
    }
}
