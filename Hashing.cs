using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;


namespace GLogic_hashing
{
    public class HashingResult
    {
        public string _fileName;
        public string _hash;
    }

    public class Hashing
    {
        //Bytes in one MB
        private const int MB = 1048576;


        //Which directory will be hashed
        private DirectoryInfo _directoryForHashing;

        //Path to file where result will be saved
        private string _outputFilePath;

        //Class which computes the SHA256 hash
        private SHA256Managed _hash = new SHA256Managed();

        //All this data will be saved at the file
        private List<HashingResult> _result = new List<HashingResult>();

        //Collect all tasks which computes files hash
        private List<Task> allTasks = new List<Task>();

        public long totalBytes { get; set; }




        public Hashing( string directoryPath, string outputFilePath )
        {
            _directoryForHashing = new DirectoryInfo( directoryPath );
            _outputFilePath = outputFilePath;
        }


        public void ComputeHash()
        {
            ComputeHashInDirectory( _directoryForHashing );
            SaveData();
        }


        private void CollectHash( FileStream fStream )
        {
            //FileInfo for get name of file without path
            FileInfo info = new FileInfo(fStream.Name);

            //Add pair: file name - file hash 
            _result.Add( new HashingResult
            {
                _fileName = info.Name,
                _hash = Encoding.Default.GetString(_hash.ComputeHash(fStream))
            });

            //Get total length for calculating performance
            totalBytes += fStream.Length;
            
        }

        private Task CollectAllFileHash( DirectoryInfo _directory )
        {
            return Task.Run( () =>
            {
                List<FileStream> fStreamList = new List<FileStream>();
                foreach ( var file in _directory.GetFiles() )
                {
                    FileStream fStream = new FileStream( file.FullName, FileMode.Open, FileAccess.Read );

                    //Save all FileStream for closing it later
                    fStreamList.Add( fStream );
                    CollectHash( fStream );
                }

                //Close all streams
                foreach ( var stream in fStreamList )
                    stream.Close();
            });
        }

        private void ComputeHashInDirectory( DirectoryInfo _directory )
        {
            //Recursively for all directories
            foreach ( DirectoryInfo directory in _directory.GetDirectories() )
            {
                ComputeHashInDirectory( directory );
            }
            

            try
            {
                //Collect all tasks
                Task t = CollectAllFileHash( _directory );
                allTasks.Add( t );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.GetType().FullName );
                Console.WriteLine( e.Message );
            }
            
        }
        

        private void SaveData()
        {
            int it = 1;
            using ( StreamWriter writer = new StreamWriter( _outputFilePath, false, Encoding.Default ) )
            {
                //Complete all tasks
                Task.WhenAll( allTasks )
                    .ContinueWith( ( Task t ) =>
                 { 
                     //Save all pairs in the file
                    foreach ( var pair in _result )
                    {
                        writer.WriteLine( $"{it}.  {pair._fileName} \t\t {pair._hash}" );
                        it++;
                    }

                    //Performance calculation (allDataSize/totalProcessorTime)
                     double procTime = Process.GetCurrentProcess().TotalProcessorTime.TotalSeconds;
                     writer.WriteLine( string.Format( "Performance: {0:0.00} MB/s (by CPU time)", (totalBytes/MB) / procTime ));

                } ).Wait();
            }
        }
    }
}
