using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;


namespace GLogic_hashing
{
    class Program
    {
        static void Main(string[] args) 
        {
            SHA256Managed sha = new SHA256Managed();
            FileStream fs = new FileStream(@"C:\Users\Aganurych\Desktop\pict.jpg", FileMode.Open);
            FileStream fsWrite = new FileStream(@"C:\Users\Aganurych\Desktop\Hash.txt", FileMode.OpenOrCreate );

            byte[] hash = sha.ComputeHash( fs );
            Task t = fsWrite.WriteAsync( hash, 0, hash.Length );
            t.Wait();

            fs.Close();
            fsWrite.Close();
            sha.Dispose();
          
        }
    }
}
