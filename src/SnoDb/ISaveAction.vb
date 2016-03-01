Imports System.IO.Compression

Public Interface ISaveAction

    Function SaveAsync(archive As ZipArchive) As Task

End Interface