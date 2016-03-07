using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SnoDb
{
    /// <summary>
    /// used for all save actions
    /// </summary>
    public interface ISaveAction
    {
        Task SaveAsync(ZipArchive archive);
    }
}
