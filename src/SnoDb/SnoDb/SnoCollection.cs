using SnoDb;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO.Compression;
using System.Collections.Concurrent;

using Newtonsoft.Json;

namespace SnoDb
{
    public class SnoCollection<T, TID> : ISnoCollection<T>
    {
        #region Properties
        public SnoDatabase Database
        { get; }

        public string Name
        { get; }
        
        public Func<T,TID> IDSelector { get; }
        public ConcurrentDictionary<TID, T> Items = new ConcurrentDictionary<TID, T>();
        public ConcurrentDictionary<TID, ISaveAction> Actions = new ConcurrentDictionary<TID, ISaveAction>();
        #endregion

        #region Constructor
        public SnoCollection(SnoDatabase database, string name, Func<T,TID> selector)
        {
            Database = database;
            Name = name;
        }
        #endregion
        #region Methods
        public void AddOrReplace(T item)
        {
            var id = IDSelector(item);
            Items[id] = item;
            var itemPath = GetItemPath(item);
            Actions[id] = new AddOrReplaceSaveAction(itemPath, item);
        }

        public string GetItemPath(T item)
        {
            var id = IDSelector(item);
            return $"{Name}/{id}.json";
        }

        public async Task LoadAsync(IEnumerable<ZipArchiveEntry> entries)
        {
            foreach(var entry in entries)
            {
                string itemJson;
                using (var reader = new StreamReader(entry.Open()))
                {
                    itemJson = await reader.ReadToEndAsync();
                }

                var item = JsonConvert.DeserializeObject<T>(itemJson);
                var id = IDSelector(item);
                Items[id] = item;

            }
        }

        public IQueryable<T> Query()
        {
            return Items.Values.AsQueryable();
        }

        public void Remove(T item)
        {
            var id = IDSelector(item);
            Items.TryRemove(id, out item);

            var itemPath = GetItemPath(item);
            Actions[id] = new RemoveSaveAction(itemPath);
        }

        public async Task SaveAsync(ZipArchive archive)
        {
           foreach(var saveAction in Actions.Values)
            {
                await saveAction.SaveAsync(archive);
            }
        }
        #endregion
    }
}
