using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CSEntropyCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<FileInfo> files = new List<FileInfo>(); //List of files to process
            foreach (string arg in args) //Reads arguments
            {
                if (arg == "c") continue; //"c" is not a file
                try
                {
                    files.AddRange(new DirectoryInfo(Path.GetDirectoryName(arg)).GetFiles(Path.GetFileName(arg), SearchOption.AllDirectories)); //Adds all files with matching search pattern
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Cannot add {arg}: {ex.Message}.");
                    Console.ResetColor();
                }
            }
            FileStream csv = null; //Creates a FileStream that can create a csv result file
            if (files.Count == 0) //No input files: display help
            {
                Console.WriteLine("CSEntropyCalculator: Calculate the entropy of files.\n" +
                    "Usage: CSEntropyCalculator [c] FILE(s) [FILE(s)]\n" +
                    "c = Export results to EntropyResults.csv\n" +
                    "\nExample: CSEntropyCalculator *.exe myfile.jpg");
            }
            else
            {
                if (args[0] == "c") //If "c" is the first argument, create a csv file to write the results
                {
                    try
                    {
                        csv = new FileStream("EntropyResults.csv", FileMode.Create, FileAccess.Write);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot create EntropyResults.csv: {ex.Message}.");
                        Console.ResetColor();
                    }
                }
            }
            foreach (FileInfo file in files) //Processes each file
            {
                try
                {
                    string cEntropy = Math.Round(CalculateEntropy(file), 3).ToString(CultureInfo.InvariantCulture); //Writes the resulting entropy in format x.xxx
                    Console.Write(cEntropy + "\t");
                    Console.WriteLine(file.FullName);
                    if (args[0] == "c") //If "c" is the first argument, also save the results to the csv file
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(cEntropy + ",\"" + file.FullName + "\"\n"); //Comma-separated
                        csv.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occurred when processing {file.FullName}: {ex.Message}.");
                    Console.ResetColor();
                }
            }
        }

        public static unsafe double CalculateEntropy(FileInfo file)
        {
            int* count = stackalloc int[256]; //Allocates a 256-byte array on the stack, for faster calculations; int[] count = new int[256]; would be safe
            //You can use Span<int> count = stackalloc int[256]; on .NET (Core), without using the unsafe keyword
            //However, Span seems to be slower, even when used below (buffer)
            for (int i = 0; i < 256; i++) //Initialize stackalloc array to 0, just in case
            {
                count[i] = 0;
            }
            var fsize = file.Length;
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                byte[] buffer = new byte[262144]; //256 KiB
                int n = 0;
                while ((n = fs.Read(buffer, 0, buffer.Length)) > 0) //Reads a chunk of the current file into the buffer
                {
                    for (int i = 0; i < n; i++)
                    {
                        count[buffer[i]]++; //Increases by 1 the amount of times the current byte has occurred
                    }
                }
            }
            double Hx = 0d; //Hx will save the result of the summation
            for (int i = 0; i < 256; i++)
            {
                if (count[i] > 0)
                {
                    double px = count[i] / (double)fsize; //p(x)
                    Hx -= px * Math.Log(px, 2); // H(x) = - Summation(p(x) * log2 (p(x)))
                }
            }
            return Hx;
        }
    }
}
