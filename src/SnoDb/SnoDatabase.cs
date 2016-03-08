using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SnoDb
{
    public class SnoDatabase
    {
        public string Name { get; }
        public string DatabaseDirectory { get; }
        public string ArchivePath { get; }
        public ConcurrentDictionary<string, ISnowCollection> collections = new ConcurrentDictionary<string, ISnowCollection>();

        public SnoDatabase(string name)
        {
            Name = name;
            DatabaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SnoDb\\Databases\\");
            Directory.CreateDirectory(ArchivePath);
            ArchivePath = Path.Combine(DatabaseDirectory, $"{name}.zip");
        }
        public SnoDatabase(string name, string databaseDirectory)
        {
            Name = name;
            DatabaseDirectory = databaseDirectory;
            Directory.CreateDirectory(ArchivePath);
            ArchivePath = Path.Combine(DatabaseDirectory, $"{name}.zip");
        }

        public ISnoCollection<T> RegisterCollection<T,TID>(Func<T,TID> selctor)
        {
            var collectionName = GetCollectionName(T);
            var collection = new SnoCollection<T,TID>(this, collectionName, selctor);
        }

        private string GetCollectionName<T>()
        {
            return GetType(T).Name

        }
    }
}
