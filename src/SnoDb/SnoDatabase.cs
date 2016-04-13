using System;
using System.Linq;
using System.Collections.Concurrent;
using System.IO;
using Ionic.Zip;

namespace Symmex.SnoDb
{
    public class SnoDatabase : IDisposable
    {
        public string Name { get; }
        public string DatabaseDirectory { get; }
        private ZipFile Archive { get; }
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

            var archivePath = Path.Combine(DatabaseDirectory, $"{Name}.zip");
            Archive = new ZipFile(archivePath);
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

        public void Load()
        {
            var entriesByCollection = (from entry in Archive.Entries
                                       let separatorIndex = entry.FileName.IndexOf('/')
                                       let collectionName = entry.FileName.Substring(0, separatorIndex)
                                       group entry by collectionName into g
                                       select new { CollectionName = g.Key, Entries = g.ToList() });

            foreach (var collectionEntries in entriesByCollection)
            {
                ISnowCollection currentCollection;
                if (Collections.TryGetValue(collectionEntries.CollectionName, out currentCollection))
                {
                    currentCollection.Load(collectionEntries.Entries);
                }
            }
        }

        public void Save()
        {
            foreach (var collection in Collections.Values)
            {
                collection.Save(Archive);
            }

            Archive.Save();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Save();
                    Archive.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
