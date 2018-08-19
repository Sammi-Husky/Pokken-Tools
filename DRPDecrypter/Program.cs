using System;
using System.IO;
using System.Linq;

namespace ConsoleTestBed
{
    unsafe class Program
    {
        private static RandomXS _rand;
        static void Main(string[] args)
        {
            Console.WriteLine("=============================");
            Console.WriteLine("DRPDecrypter v0.8_h1");
            Console.WriteLine("Copyright(c) 2016 Sammi Husky");
            Console.WriteLine("Licensed under MIT License");
            Console.WriteLine("=============================\n");
            if (args.Length <= 0 || args.Length > 1)
            {
                Console.WriteLine("Usage:\n\tDRPDecrypt <drp file>");
                return;
            }
            Console.WriteLine($"Decrypting {args[0]}..");
            File.WriteAllBytes(args[0] + ".dec", Decrypt(args[0]));
        }

        public static byte[] Decrypt(string path)
        {
            int[] filedata = null;
            using (var stream = File.Open(path, FileMode.Open))
            {
                filedata = new int[stream.Length / 4];
                using (var reader = new BinaryReader(stream))
                {
                    int size = (int)stream.Length;
                    int words = size >> 2;
                    int xorval = 0;
                    stream.Seek(0x1C, SeekOrigin.Begin);
                    int SEED = reader.ReadInt32().Reverse();
                    _rand = new RandomXS(SEED);
                    stream.Seek(0, SeekOrigin.Begin);
                    if (size % 8 == 0) goto Dword_loop;
                    else if (size == 0) return null;
                    else if (size / 8 <= 0) goto BYTE_loop;
                    else;//goto default_loop

                    #region DWORD Loop
                    Dword_loop:
                    if (words == 0)
                        goto loops_end;
                    else if (words / 8 <= 0)
                        goto Word_Loop;
                    else
                        for (int i = 0; i < words >> 3; i++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                var randInt = _rand.GetInt();
                                var XOR = reader.ReadInt32().Reverse() ^ randInt;
                                var val = XOR ^ xorval;
                                xorval = (randInt << 13) & unchecked((int)0x80000000);
                                filedata[x + (i * 8)] = val.Reverse();
                            }
                        }
                    #endregion
                    #region Word Loop
                    Word_Loop:
                    if ((words & 7) <= 0)
                        goto loops_end;
                    int offset = (size >> 2 >> 3 << 3 << 2);
                    stream.Seek(offset, SeekOrigin.Begin);

                    for (int i = 0; i < (words & 7); i++)
                    {
                        var randInt = _rand.GetInt();
                        var XOR = reader.ReadInt32().Reverse() ^ randInt;
                        var val = XOR ^ xorval;
                        xorval = (randInt << 13) & unchecked((int)0x80000000);
                        filedata[offset / 4 + i] = val.Reverse();
                    }
                    goto loops_end;
                    #endregion
                    #region BYTE Loop
                    BYTE_loop:
                    if ((size & 7) == 0)
                        goto func_end;
                    stream.Seek(4, SeekOrigin.Begin);
                    for (int i = 4; i < (size & 7); i++)
                    {
                        byte[] data = BitConverter.GetBytes(filedata[i.RoundDown(4)]);
                        var b = reader.ReadByte();
                        var shifted = (b >> 3) | (b << 32 - 3);
                        var val = b ^ shifted;
                        data[i] = (byte)val;
                    }
                    #endregion
                    loops_end:
                    filedata[7] = SEED.Reverse();
                }
            }
            func_end:
            byte[] result = new byte[filedata.Length * sizeof(int)];
            Buffer.BlockCopy(filedata, 0, result, 0, result.Length);
            return result;
        }
    }
    public class RandomXS
    {
        public RandomXS(int seed)
        {
            int init = 0x41C64E6D;
            _data = new int[4];
            _data[0] = (seed * init) + 0x3039;
            _data[1] = (_data[0] * init) + 0x3039;
            _data[2] = (_data[1] * init) + 0x3039;
            _data[3] = (_data[2] * init) + 0x3039;
        }
        private int[] _data;
        public int GetInt()
        {
            var XOR_INT = _data[0] ^ (_data[0] >> 0x13) ^
                            _data[3] ^ (_data[3] << 11) ^
                            ((_data[3] ^ (_data[3] << 11)) >> 8);

            int tmp = _data[1];
            _data[1] = _data[0];
            _data[3] = _data[2];
            _data[2] = tmp;
            _data[0] = XOR_INT;
            return XOR_INT & 0x7FFFFFFF;
        }
    }
}
