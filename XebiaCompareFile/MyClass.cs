using System;
using System.IO;
using System.Activities;
using System.ComponentModel;
using System.Security.Cryptography;

namespace XebiaCustomActivity
{
    public class XebiaCompareFile : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FileA { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FileB { get; set; }

        [Category("Output")]
        public OutArgument<bool> Condition { get; set; }

        // Method to calculate MD5Hash
        static string MD5Hash(string FilePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FilePath))
                {   // Calculate MD5Hash
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            // Define variable for byte.
            int FileAPathbyte;
            int FileBPathbyte;
            FileStream fs1 = null;
            FileStream fs2 = null;

            //Read Filepath
            var FileAPath = FileA.Get(context);
            var FileBPath = FileB.Get(context);

            // Determine if the same file was referenced two times.
            if (FileAPath == FileBPath)
            {
                // Return true to indicate that the files are the same.
                Condition.Set(context, true);
            }

            // Open the one files.
            if (FileAPath != null)
            {
                fs1 = File.Open(FileAPath, FileMode.Open);
            }
            // Open the two files.
            if (FileBPath != null)
            {
                fs2 = File.Open(FileBPath, FileMode.Open);
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // FileAPath is reached.
            do
            {
                // Read one byte from each file.
                FileAPathbyte = fs1.ReadByte();
                FileBPathbyte = fs2.ReadByte();
            }
            while ((FileAPathbyte == FileBPathbyte) && (FileAPathbyte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "FileAPathbyte" is 
            // equal to "FileBPathbyte" at this point only if the files are 
            // the same.
            if ((FileAPathbyte - FileBPathbyte) == 0)
            {
                // Return true to indicate that the files are the same.
                Condition.Set(context, true);
            }

            string MD5A = MD5Hash(FileAPath);
            string MD5B = MD5Hash(FileBPath);

            if (MD5A == MD5B)
            {
                // Return true to indicate that the files are the same.
                Condition.Set(context, true);
            }
            else
            {
                // Return false to indicate that the files are not same.
                Condition.Set(context, false);
            }

        }

    }
}