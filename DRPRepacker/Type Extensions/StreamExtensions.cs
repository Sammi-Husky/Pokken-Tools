// Copyright (c) Sammi Husky. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.IO
{
    public static class StreamExtensions
    {
        //=========================================================//
        //==================== Binary Reader ======================//
        //=========================================================//
        public static int ReadInt32(this BinaryReader reader, Endianness endian)
        {
            if (endian == Endianness.Big)
                return reader.ReadBint32();
            else
                return reader.ReadInt32();
        }
        public static uint ReadUInt16(this BinaryReader reader, Endianness endian)
        {
            if (endian == Endianness.Big)
                return reader.ReadBuint16();
            else
                return reader.ReadUInt16();
        }
        public static uint ReadUInt32(this BinaryReader reader, Endianness endian)
        {
            if (endian == Endianness.Big)
                return reader.ReadBuint32();
            else
                return reader.ReadUInt32();
        }
        public static float ReadSingle(this BinaryReader reader, Endianness endian)
        {
            return reader.ReadSingle().Reverse();
        }

        public static int ReadBint32(this BinaryReader reader)
        {
            return reader.ReadInt32().Reverse();
        }
        public static short ReadBint16(this BinaryReader reader)
        {
            return reader.ReadInt16().Reverse();
        }
        public static ushort ReadBuint16(this BinaryReader reader)
        {
            return reader.ReadUInt16().Reverse();
        }
        public static uint ReadBuint32(this BinaryReader reader)
        {
            return reader.ReadUInt32().Reverse();
        }
        public static float ReadBfloat(this BinaryReader reader)
        {
            return reader.ReadSingle().Reverse();
        }

        public static string ReadStringNT(this BinaryReader reader)
        {
            string str = "";
            char ch;
            while ((int)(ch = reader.ReadChar()) != 0)
                str = str + ch;
            return str;
        }
        //=========================================================//
        //==================== Binary Writer ======================//
        //=========================================================//
        public static void Write(this BinaryWriter writer, int value, Endianness endian)
        {
            if (endian == Endianness.Big)
                writer.Write(value.Reverse());
            else
                writer.Write(value);
        }
        public static void Write(this BinaryWriter writer, short value, Endianness endian)
        {
            if (endian == Endianness.Big)
                writer.Write(value.Reverse());
            else
                writer.Write(value);
        }
        public static void Write(this BinaryWriter writer, float value, Endianness endian)
        {
            if (endian == Endianness.Big)
                writer.Write(value.Reverse());
            else
                writer.Write(value);
        }
        public static void Write(this BinaryWriter writer, uint value, Endianness endian)
        {
            if (endian == Endianness.Big)
                writer.Write(value.Reverse());
            else
                writer.Write(value);
        }
        public static void Write(this BinaryWriter writer, ushort value, Endianness endian)
        {
            if (endian == Endianness.Big)
                writer.Write(value.Reverse());
            else
                writer.Write(value);
        }

        /// <summary>
        /// Writes a null-terminated string to the data stream.
        /// </summary>
        /// <param name="writer">Data stream to write to</param>
        /// <param name="value">String to write to the stream</param>
        public static void WriteStringNT(this BinaryWriter writer, string value)
        {
            WriteStringNT(writer, value, 1);
        }
        /// <summary>
        /// Writes a null-terminated string to the data stream, padding the output with null bytes until total written bytes == blockSize
        /// </summary>
        /// <param name="writer">Data Stream to write to</param>
        /// <param name="value">String to write to the stream</param>
        /// <param name="blockSize">The total size of the block to write. will pad with null bytes up to this number.</param>
        public static void WriteStringNT(this BinaryWriter writer, string value, int blockSize)
        {
            long pos = writer.BaseStream.Position;
            writer.Write(value.ToCharArray());

            while ((writer.BaseStream.Position - pos) < blockSize)
                writer.Write((byte)0);
        }
    }
}
