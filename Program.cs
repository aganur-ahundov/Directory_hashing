using System;

namespace GLogic_hashing
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Hashing hash = new Hashing( args[0], args[1] );
                hash.ComputeHash();
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }

        }
    }
}
