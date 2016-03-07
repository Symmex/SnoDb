using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace SnoDb
{
    /// <summary>
    /// Handles operations to add or replace a new entry in the zip archive  
    /// </summary>
    public class AddOrReplaceSaveAction : ISaveAction
    {
        #region Properties
        public string ItemPath { get; }
        public string ItemJson { get; }
        #endregion
        #region Constructor
        public AddOrReplaceSaveAction(string path, object item)
        {
            ItemPath = path;
            ItemJson = JsonConvert.SerializeObject(item);
        }
        #endregion
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
