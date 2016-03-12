using System.Linq;
using System.Collections.Generic;
using Ionic.Zip;

namespace Symmex.SnoDb
{
    /// <summary>
    /// Non generic version of ISnoCollection
    /// </summary>
    public interface ISnowCollection
    {
        SnoDatabase Database { get; }
        string Name { get; }

        void Load(IEnumerable<ZipEntry> entries);
        void Save(ZipFile archive);
    }

    /// <summary>
    /// Generic version of ISnoCollection
    /// </summary>
    /// <typeparam name="t"></typeparam>
    public interface ISnoCollection<T> : ISnowCollection
    {
        void AddOrReplace(T item);
        void Remove(T item);
        IQueryable<T> Query();
        string GetItemPath(T item);
    }

}

   
