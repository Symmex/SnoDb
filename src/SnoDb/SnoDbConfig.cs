using System;
using System.IO;

namespace Symmex.SnoDb
{
    public static class SnoDbConfig
    {
        public static string DefaultDatabaseDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"SnoDb\Databases\");
    }
}