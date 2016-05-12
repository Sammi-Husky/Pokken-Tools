using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRPExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = Console.ReadLine();
            using(var stream = File.Open(s, FileMode.Open))
            {
                using(var reader = new BinaryReader(stream))
                {

                }
            }
        }
    }
}
