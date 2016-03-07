using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public async Task SaveAsync(ZipArchive archive)
        {
            var entry = archive.GetEntry(ItemPath);
            if(entry != null)
            {
                entry.Delete();
            }

            entry =  archive.CreateEntry(ItemPath);
            using (var writer = new StreamWriter(entry.Open()))
            {
               await  writer.WriteAsync(ItemJson);
            }
        }
    }
}
