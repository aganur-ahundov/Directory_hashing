using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace GLogic_hashing
{
    public class Hashing
    {
        //Which directory will be hashed
        private DirectoryInfo _directoryForHashing;

        //File for results
        private FileStream _outputFile;

        private SHA256Managed _hash = new SHA256Managed();

        public Hashing( string directoryPath, string outputFilePath )
        {
            _directoryForHashing = new DirectoryInfo( directoryPath );
            _outputFile = new FileStream( outputFilePath, FileMode.OpenOrCreate );
        }

        public void ComputeHash()
        {
            //_directoryForHashing.GetDirectories().AsParallel
            ComputeDirectory( _directoryForHashing );
        }

        private byte[] ComputeFileHash( string path )
        {
            FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read );
            byte[] hashBytes = _hash.ComputeHash( fs );

            fs.Close();

            return hashBytes;
        }

        private void ComputeDirectory( DirectoryInfo _directory )
        {
            _directory.GetFiles().AsParallel().ForAll( f => 
            {
                byte[] fileNameBytes = ASCIIEncoding.UTF8.GetBytes( " " + f.Name );
                byte[] hashBytes = ComputeFileHash( f.FullName );
                byte[] result = hashBytes.Concat( fileNameBytes ).ToArray();

                _outputFile.WriteAsync( result, 0, result.Length );
            }
            );

            //_directory.GetDirectories().AsParallel
            foreach ( DirectoryInfo directory in _directory.GetDirectories() )
            {
                ComputeDirectory( directory );
            }
        }
    }
}
