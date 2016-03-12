using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;

namespace SnoDb
{
    public class SnoDatabase
    {
        public string Name { get; }
        public string DatabaseDirectory { get; }
        private string ArchivePath { get; }
        private ConcurrentDictionary<string, ISnowCollection> Collections { get; } = new ConcurrentDictionary<string, ISnowCollection>();

        public SnoDatabase(string name)
            : this(name, SnoDbConfig.DefaultDatabaseDirectory)
        {
        }

        public SnoDatabase(string name, string databaseDirectory)
        {
            Name = name;
            DatabaseDirectory = databaseDirectory;

            Directory.CreateDirectory(DatabaseDirectory);
            ArchivePath = Path.Combine(DatabaseDirectory, $"{Name}.zip");
        }

        private string GetCollectionName<T>()
        {
            return typeof(T).Name;
        }

        public ISnoCollection<T> RegisterCollection<T, TId>(Func<T, TId> idSelector)
        {
            var collectionName = GetCollectionName<T>();
            var collection = new SnoCollection<T, TId>(this, collectionName, idSelector);
            Collections.TryAdd(collectionName, collection);

            return collection;
        }

        public ISnoCollection<T> GetCollection<T>()
        {
            var collectionName = GetCollectionName<T>();
            var collection = Collections[collectionName];

            return (ISnoCollection<T>)collection;
        }

        public async Task LoadAsync()
        {
            if (!File.Exists(ArchivePath))
            {
                return;
            }

            using (var archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Read))
            {
                var entriesByCollection = (from entry in archive.Entries
                                           let separatorIndex = entry.FullName.IndexOf('/')
                                           let collectionName = entry.FullName.Substring(0, separatorIndex)
                                           group entry by collectionName into g
                                           select new { CollectionName = g.Key, Entries = g.ToList() });

                foreach (var collectionEntries in entriesByCollection)
                {
                    ISnowCollection currentCollection;
                    if (Collections.TryGetValue(collectionEntries.CollectionName, out currentCollection))
                    {
                        await currentCollection.LoadAsync(collectionEntries.Entries);
                    }
                }
            }
        }

        public async Task SaveAsync()
        {
            using (var archive = ZipFile.Open(ArchivePath, ZipArchiveMode.Update))
            {
                foreach (var collection in Collections.Values)
                {
                    await collection.SaveAsync(archive);
                }
            }
        }
    }
}
