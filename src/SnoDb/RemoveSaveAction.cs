using Ionic.Zip;

namespace Symmex.SnoDb
{
    public class RemoveSaveAction : ISaveAction
    {
        public string ItemPath { get; }

        public RemoveSaveAction(string itemPath)
        {
            ItemPath = itemPath;
        }

        public void Save(ZipFile archive)
        {
            if(archive.ContainsEntry(ItemPath))
                archive.RemoveEntry(ItemPath);
        }
    }
}
