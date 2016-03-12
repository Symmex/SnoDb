using Ionic.Zip;

namespace SnoDb
{
    /// <summary>
    /// used for all save actions
    /// </summary>
    public interface ISaveAction
    {
        void Save(ZipFile archive);
    }
}
