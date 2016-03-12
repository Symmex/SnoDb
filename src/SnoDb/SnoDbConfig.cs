using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SnoDb
{
    static class SnoDbConfig
    {
        public static string DefaultDatabaseDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"SnoDb\Databases\");
    }
}