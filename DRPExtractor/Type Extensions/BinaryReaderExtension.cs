using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System.IO
{
    public static class BinaryReaderExtension
    {
        public static string ReadTerminatedString(this BinaryReader reader)
        {
            string s = "";

            char c;
            while ((c = reader.ReadChar()) != '\0')
                s += c;
            return s;
        }
    }
}
