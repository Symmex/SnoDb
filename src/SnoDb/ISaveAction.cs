using Ionic.Zip;

namespace Symmex.SnoDb
{
    /// <summary>
    /// used for all save actions
    /// </summary>
    public interface ISaveAction
    {
        void Save(ZipFile archive);
    }
}
