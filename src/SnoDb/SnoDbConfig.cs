using System;
using System.IO;
using Newtonsoft.Json;

namespace Symmex.SnoDb
{
    public static class SnoDbConfig
    {
        public static string DefaultDatabaseDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"SnoDb\Databases\");
        public static JsonSerializerSettings DefaultSerializerSettings { get; set; } = new JsonSerializerSettings();
        public static PersistenceMode PersistenceMode { get; set; }
    }
}