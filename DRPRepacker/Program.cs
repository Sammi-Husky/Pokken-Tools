using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRPRepacker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=============================");
            Console.WriteLine("DRPRepacker v1.0");
            Console.WriteLine("Copyright(c) 2018 Sammi Husky");
            Console.WriteLine("Licensed under MIT License");
            Console.WriteLine("=============================\n");
            if (args.Length == 2 && Directory.Exists(args[1]))
            {
                var drpfile = new DRPFile(args[0]);
                foreach(string file in Directory.EnumerateFiles(args[1]))
                {
                    if (drpfile.Entries.ContainsKey(Path.GetFileName(file)))
                    {
                        drpfile.ReplaceFile(Path.GetFileName(file), File.ReadAllBytes(file));
                    }
                }
                drpfile.WriteFile("repacked.drp");
            }
            else
            {
                Console.WriteLine("Usage:\n\tDRPRepacker <drp file to patch> <folder containing files>");
                return;
            }
        }
    }
}
