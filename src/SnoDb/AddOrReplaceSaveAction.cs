using System.IO;
using Newtonsoft.Json;
using Ionic.Zip;

namespace SnoDb
{
    /// <summary>
    /// Handles operations to add or replace a new entry in the zip archive  
    /// </summary>
    public class AddOrReplaceSaveAction : ISaveAction
    {
        public string ItemPath { get; }
        public string ItemJson { get; }

        public AddOrReplaceSaveAction(string path, object item)
        {
            ItemPath = path;
            ItemJson = JsonConvert.SerializeObject(item);
        }

        public void Save(ZipFile archive)
        {
            archive.RemoveEntry(ItemPath);
            var entry =  archive.AddEntry(ItemPath, ItemJson);
        }
    }
}
