using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace SnoDb
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
        }

        public async Task LoadAsync(IEnumerable<ZipArchiveEntry> entries)
        {
            foreach (var entry in entries)
            {
                string itemJson;
                using (var reader = new StreamReader(entry.Open()))
                {
                    itemJson = await reader.ReadToEndAsync();
                }

                var item = JsonConvert.DeserializeObject<T>(itemJson);
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
        }

        public async Task SaveAsync(ZipArchive archive)
        {
            foreach (var saveAction in SaveActions.Values)
            {
                await saveAction.SaveAsync(archive);
            }
        }
    }
}
