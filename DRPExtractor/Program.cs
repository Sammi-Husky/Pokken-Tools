using System;
using System.IO;
using System.IO.Compression;
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
            Console.WriteLine("=============================");
            Console.WriteLine("DRPExtractor v0.8");
            Console.WriteLine("Copyright(c) 2016 Sammi Husky");
            Console.WriteLine("Licensed under MIT License");
            Console.WriteLine("=============================\n");
            if (args.Length <= 0 || args.Length > 1)
            {
                Console.WriteLine("Usage:\n\tDRPE <drp file>");
                return;
            }

            using (var stream = File.Open(args[0], FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    stream.Seek(0x60, SeekOrigin.Begin);
                    Directory.CreateDirectory(args[0].Remove(args[0].IndexOf('.')));
                    while (stream.Position != stream.Length)
                    {
                        var BaseAddr = stream.Position;
                        var name = reader.ReadTerminatedString();
                        stream.Position = BaseAddr + 0x44;
                        var next = reader.ReadInt32().Reverse();
                        stream.Position = BaseAddr + 0x50;
                        var size = reader.ReadInt32().Reverse();
                        stream.Position = BaseAddr + 0x66;

                        using (var originStream = new MemoryStream(reader.ReadBytes(size)))
                        {
                            using (var destStream = File.Create(Path.Combine(args[0].Remove(args[0].IndexOf('.')), name)))
                            {
                                using (var decompStream = new DeflateStream(originStream, CompressionMode.Decompress))
                                {
                                    decompStream.CopyTo(destStream);
                                }
                            }
                        }
                        stream.Position = BaseAddr + next;
                    }
                }
            }
        }
    }
}
