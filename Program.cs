using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using GLogic_hashing;

namespace GLogic_hashing
{
    class Program
    {
        static void Main( string[] args ) 
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Hashing hash = new Hashing(@"C:\Users\Aganurych\Desktop\Test", @"C:\Users\Aganurych\Desktop\Output.txt");
            try
            {
                //Hashing hash = new Hashing( args[0], args[1] );
                hash.ComputeHash();

            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
            sw.Stop();
            Console.WriteLine( $"Total Time: {sw.ElapsedMilliseconds} ms" );
            Console.WriteLine( $"Total Processor Time: { Process.GetCurrentProcess().TotalProcessorTime.Milliseconds } ms");
            Console.WriteLine( $"User Processor Time: { Process.GetCurrentProcess().UserProcessorTime.Milliseconds } ms" );
            Console.WriteLine( $"Absolute: { hash.totalTime } ms" );
        }
    }
}
