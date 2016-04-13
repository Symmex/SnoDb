using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Ionic.Zip;

namespace Symmex.SnoDb
{
    public class SnoCollection<T, TId> : ISnoCollection<T>
    {
        public SnoDatabase Database { get; }
        public string Name { get; }

        private Func<T, TId> IdSelector { get; }
        private ConcurrentDictionary<TId, T> Items = new ConcurrentDictionary<TId, T>();
        private ConcurrentDictionary<TId, ISaveAction> SaveActions = new ConcurrentDictionary<TId, ISaveAction>();

        public SnoCollection(SnoDatabase database, string name, Func<T, TId> idSelector)
        {
            Database = database;
            Name = name;
            IdSelector = idSelector;
        }

        public void Load(IEnumerable<ZipEntry> entries)
        {
            foreach (var entry in entries)
            {
                string itemJson;
                using (var reader = new StreamReader(entry.OpenReader()))
                {
                    itemJson = reader.ReadToEnd();
                }

                var item = JsonConvert.DeserializeObject<T>(itemJson, SnoDbConfig.DefaultSerializerSettings);
                var id = IdSelector(item);
                Items[id] = item;
            }
        }

        public IQueryable<T> Query()
        {
            return Items.Values.AsQueryable();
        }

        public void AddOrReplace(T item)
        {
            var id = IdSelector(item);
            Items[id] = item;
            var itemPath = GetItemPath(item);
            SaveActions[id] = new AddOrReplaceSaveAction(itemPath, item);

            if (SnoDbConfig.PersistenceMode == PersistenceMode.Instant)
                Database.Save();
        }

        public string GetItemPath(T item)
        {
            var id = IdSelector(item);
            return $"{Name}/{id}.json";
        }

        public void Remove(T item)
        {
            var id = IdSelector(item);
            Items.TryRemove(id, out item);

            var itemPath = GetItemPath(item);
            SaveActions[id] = new RemoveSaveAction(itemPath);

            if (SnoDbConfig.PersistenceMode == PersistenceMode.Instant)
                Database.Save();
        }

        public void Save(ZipFile archive)
        {
            foreach (var saveAction in SaveActions.Values)
            {
                saveAction.Save(archive);
            }

            SaveActions.Clear();
        }
    }
}
