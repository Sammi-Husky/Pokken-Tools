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
                        stream.Position = BaseAddr + 0x4A;
                        var count = reader.ReadInt16().Reverse();
                        stream.Position = BaseAddr + 0x50;
                        var sizes = new int[count];
                        for (int i = 0; i < count; i++)
                            sizes[i] = reader.ReadInt32().Reverse();

                        stream.Position = BaseAddr + 0x60;
                        for (int i = 0; i < count; i++)
                        {
                            var newname = name;
                            if (count > 1)
                                newname += $"[{i}]";

                            stream.Seek(0x06, SeekOrigin.Current);
                            using (var originStream = new MemoryStream(reader.ReadBytes(sizes[i] - 6)))
                            {
                                using (var destStream = File.Create(Path.Combine(args[0].Remove(args[0].IndexOf('.')), newname)))
                                {
                                    using (var decompStream = new DeflateStream(originStream, CompressionMode.Decompress))
                                    {
                                        decompStream.CopyTo(destStream);
                                    }
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
