using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRPRepacker
{
    public class DRPFile
    {
        public DRPFile(string filepath)
        {
            Entries = new Dictionary<string, DRPEntry>();
            Read(filepath);
        }
        public const int HEADER_SIZE = 0x20;

        public string   Filename { get; set; }
        public short    Unk1 { get; set; }
        public short    FileCount { get { return (short)Entries.Count; } }
        public uint     CRC { get; set; }
        public uint     RANDXS_SEED { get; set; }

        public Dictionary<string, DRPEntry> Entries { get; set; }

        public void Read(string filepath)
        {
            using (var stream = File.Open(filepath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    CRC = reader.ReadBuint32();

                    stream.Seek(0x14, SeekOrigin.Begin);
                    Unk1 = reader.ReadBint16();
                    int _filecount = reader.ReadBint16();

                    stream.Seek(0x1C, SeekOrigin.Begin);
                    RANDXS_SEED = reader.ReadBuint32();

                    stream.Seek(0x60, SeekOrigin.Begin); // Data start

                    for (int i = 0; i < _filecount; i++)
                    {
                        var entry = new DRPEntry(reader);
                        Entries.Add(entry.Filename, entry);
                    }
                }
            }
        }
        public bool ReplaceFile(string file, byte[] data)
        {
            if (!Entries.ContainsKey(file))
                return false;

            int partNum = 0;
            if (file.Contains("["))
            {
                partNum = int.Parse(file.Substring(file.IndexOf("["), file.IndexOf("]")));
                file = file.Remove(file.IndexOf("_["));
            }
            DRPEntry entry = Entries[file];
            byte[] cdata = Util.Compress(data);
            entry.Files[partNum] = new DRPFileEntry(cdata, data.Length);
            return true;
        }
        public void Rebuild()
        {
            foreach(var entry in Entries)
            {
                entry.Value.Rebuild();
            }
        }
        public void WriteFile(string filepath)
        {
            Rebuild();
            using(var stream = File.Open(filepath, FileMode.OpenOrCreate))
            {
                using(var writer = new BinaryWriter(stream))
                {
                    writer.Write(CRC, Endianness.Big);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(Unk1, Endianness.Big);
                    writer.Write(FileCount, Endianness.Big);
                    writer.Write(0);
                    writer.Write(RANDXS_SEED, Endianness.Big);
                    for(int i = 0; i < 0x40; i++)
                    {
                        writer.Write((byte)0);
                    }

                    foreach(var entry in Entries)
                    {
                        entry.Value.Write(writer);
                    }
                }
            }
        }
    }
    public class DRPEntry
    {
        public DRPEntry(BinaryReader reader)
        {
            Read(reader);
        }
        public DRPEntry()
        {

        }

        public const int HEADER_SIZE = 0x60;

        public string   Filename { get; set; }
        public short    FiletypeA { get; set; }
        public short    FiletypeB { get; set; }
        public int      EntrySize { get; set; }
        public short    Unk { get; set; }
        public short    Subfiles { get; set; }
        public int      Padding { get; set; }

        public int[] FileEntrySizes = new int[4];
        public DRPFileEntry[] Files { get; set; }

        public void Read(BinaryReader reader)
        {
            long basaddr    = reader.BaseStream.Position;

            Filename        = reader.ReadStringNT();
            reader.BaseStream.Seek(basaddr + 0x40, SeekOrigin.Begin);
            FiletypeA       = reader.ReadBint16();
            FiletypeB       = reader.ReadBint16();
            EntrySize       = reader.ReadBint32();
            Unk             = reader.ReadBint16();
            Subfiles        = reader.ReadBint16();
            Padding         = reader.ReadBint32();
            Files           = new DRPFileEntry[Subfiles];

            for (int i = 0; i < 4; i++)
            {
                FileEntrySizes[i] = reader.ReadBint32();
            }

            for (int i = 0; i < Subfiles; i++)
            {
                int decompLen = reader.ReadBint32();
                Files[i] = new DRPFileEntry(reader.ReadBytes(FileEntrySizes[i] - 4), decompLen); // -4 to account for decompLen field included in  FileEntry
            }
        }

        public void Rebuild()
        {
            int totalSize = 0;
            for (int i = 0; i < Files.Length; i++)
            {
                FileEntrySizes[i] = Files[i].SIZE;
                totalSize += Files[i].SIZE;
            }
            EntrySize = (totalSize += DRPEntry.HEADER_SIZE);
            Subfiles = (short)Files.Count(x => x.SIZE != 0);
        }
        public void Write(BinaryWriter writer)
        {
            writer.WriteStringNT(Filename, 0x40);
            writer.Write(FiletypeA, Endianness.Big);
            writer.Write(FiletypeB, Endianness.Big);
            writer.Write(EntrySize, Endianness.Big);
            writer.Write(Unk, Endianness.Big);
            writer.Write(Subfiles, Endianness.Big);
            writer.Write(0x0);

            for (int i = 0; i < FileEntrySizes.Length; i++)
                writer.Write(FileEntrySizes[i], Endianness.Big);

            foreach(var file in Files)
            {
                writer.Write(file.DecompSize, Endianness.Big);
                writer.Write(file.Data);
            }
        }
    }
    public struct DRPFileEntry
    {
        public DRPFileEntry(byte[] compData, int decompSize)
        {
            DecompSize = decompSize;
            Data = compData;
        }
        public int DecompSize;
        public byte[] Data;

        public int SIZE { get { return Data.Length + 4; } }
    }
}
