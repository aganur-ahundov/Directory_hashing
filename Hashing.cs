using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
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


        public int totalTime { get; set; }
        public long totalBytes { get; set; }


        public Hashing( string directoryPath, string outputFilePath )
        {
            _directoryForHashing = new DirectoryInfo( directoryPath );
            _outputFilePath = outputFilePath;
        }


        public void ComputeHash()
        {
            //CLEAR HERE
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            ComputeDirectory( _directoryForHashing );
            sw1.Stop();
            Console.WriteLine( $"Compute Directory: {sw1.ElapsedMilliseconds} ms" );

            sw1.Restart();
            var tmp = allTasks.Where( t => t.Status == TaskStatus.RanToCompletion ).Count();
            Task.WaitAll( allTasks.ToArray() );
            sw1.Stop();
            Console.WriteLine( $"Wait All: { sw1.ElapsedMilliseconds } ms" );

            sw1.Restart();
            SaveData();
            sw1.Stop();
            Console.WriteLine( $"Save Data: { sw1.ElapsedMilliseconds } ms" );

        }


        private Task  ComputeFileHash( FileStream fStream )
        {
           return Task.Run( () => 
           {
               //Get total length for calculating performance
               totalBytes += fStream.Length;

               //FileInfo for get name of file without path
               FileInfo info = new FileInfo( fStream.Name );

               //Add pair: file name - file hash 
               _result.Add( new HashingResult
               {
                   _fileName = info.Name,
                   _hash = Encoding.Default.GetString( _hash.ComputeHash( fStream ))
               } );

               //?????
               totalTime += Process.GetCurrentProcess().TotalProcessorTime.Milliseconds;
           });
        }


        private void ComputeDirectory( DirectoryInfo _directory )
        {
            List<Task> taskList = new List<Task>();
            List<FileStream> fStreamList = new List<FileStream>();

            try
            {
                foreach ( var file in _directory.GetFiles() )
                {
                    FileStream fStream = new FileStream( file.FullName, FileMode.Open, FileAccess.Read );

                    //Save all FileStream for closing it later
                    fStreamList.Add( fStream );

                    
                    Task task = ComputeFileHash( fStream );
                    taskList.Add( task );
                }

                //Save all tasks for checking if all of them are completed
                allTasks.AddRange( taskList );
                
                //Close FileStreams, after all tasks are completed
                Task.WhenAll( taskList.ToArray() ).ContinueWith( ( Task t ) =>
                {
                    foreach ( var stream in fStreamList )
                      stream.Close();
                });

            }
            catch ( Exception e )
            {
                Console.WriteLine( e.GetType().FullName );
                Console.WriteLine( e.Message );
            }


            //Recursively for all directories
            foreach ( DirectoryInfo directory in _directory.GetDirectories() )
            {
                ComputeDirectory( directory );
            }
        }

        private void SaveData()
        {
            int it = 1;
            using ( StreamWriter writer = new StreamWriter( _outputFilePath, false, Encoding.Default ) )
            {
                //Save all pairs in the file
                foreach ( var pair in _result )
                {
                    writer.WriteLine( $"{it}.  {pair._fileName} \t\t {pair._hash}" );
                    it++;
                }
            }
        }
    }
}
