using System.IO.Compression;
using System.Threading.Tasks;

namespace SnoDb
{
    public class RemoveSaveAction : ISaveAction
    {
        public string ItemPath { get; }

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
    }
}
