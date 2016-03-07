using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoDb
{
    public class RemoveSaveAction : ISaveAction
    {
        #region Properties
        public string ItemPath { get; }
        #endregion
        #region Constructor
        public RemoveSaveAction(string itemPath)
        {
            ItemPath = itemPath;
        }

        public Task SaveAsync(ZipArchive archive)
        {
            var entry = archive.GetEntry(ItemPath);
            if (entry != null)
                entry.Delete();

            return Task.CompletedTask;
        }
        #endregion


    }
}
