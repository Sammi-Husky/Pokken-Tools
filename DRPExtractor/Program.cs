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
            Console.WriteLine("Copyright(c) 2018 Sammi Husky");
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
                    stream.Seek(0x14, SeekOrigin.Begin);
                    Directory.CreateDirectory(args[0].Remove(args[0].IndexOf('.')));
                    short unk1 = reader.ReadInt16().Reverse();
                    short filecount = reader.ReadInt16().Reverse();
                    stream.Seek(0x60, SeekOrigin.Begin);
                    for (int i = 0; i < filecount; i++)
                    {
                        long baseaddr = stream.Position;

                        string name = reader.ReadTerminatedString();
                        stream.Seek(baseaddr + 0x40, SeekOrigin.Begin);

                        short filetypeA = reader.ReadInt16().Reverse();
                        short filetpyeB = reader.ReadInt16().Reverse();
                        int ChunkSize = reader.ReadInt32().Reverse();
                        short unk = reader.ReadInt16().Reverse();
                        short subfiles = reader.ReadInt16().Reverse();

                        stream.Seek(0x4, SeekOrigin.Current); // Padding

                        int[] offsets = new int[4];
                        int[] sizes = new int[4];

                        for (int x = 0; x < 4; x++)
                            sizes[x] = reader.ReadInt32().Reverse();

                        int decompSize = reader.ReadInt32().Reverse();

                        // Data starts here
                        for (int x = 0; x < subfiles; x++)
                        {
                            byte[] decompData = ZLibNet.ZLibCompressor.DeCompress(reader.ReadBytes(sizes[x] - 4)); // -4 to account for DecompLen Field in FileEntry
                            if (decompData.Length != decompSize)
                                throw new Exception("Decompressed length does not match length in header!");

                            if (x != 0)
                                name += $"_[{x}]";

                            Console.WriteLine(name);
                            using (var ostream = File.Create(Path.Combine(args[0].Remove(args[0].IndexOf('.')), name)))
                            {
                                ostream.Write(decompData, 0, decompSize);
                            }
                        }
                    }
                }
            }
        }
    }
}
