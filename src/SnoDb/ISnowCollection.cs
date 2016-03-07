using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SnoDb
{
    /// <summary>
    /// Non generic version of ISnoCollection
    /// </summary>
    public interface ISnowCollection
    {
        SnoDatabase Database { get; }
        string Name { get; }

        Task LoadAsync(IEnumerable<ZipArchiveEntry> entries);
        Task SaveAsync(ZipArchive archive);
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

   
