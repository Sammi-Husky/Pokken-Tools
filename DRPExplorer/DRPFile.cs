using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRPExplorer
{
    class DRPFile
    {
        public string Filename { get; set; }
        public uint CRC { get; set; }
        public uint ENCRYPTION_SEED { get; set; }

        public DRPEntry Entries { get; set; }
    }
}
