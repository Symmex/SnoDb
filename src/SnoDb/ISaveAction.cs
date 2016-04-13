using Ionic.Zip;

namespace Symmex.SnoDb
{
    /// <summary>
    /// Used for all save actions
    /// </summary>
    public interface ISaveAction
    {
        void Save(ZipFile archive);
    }
}
