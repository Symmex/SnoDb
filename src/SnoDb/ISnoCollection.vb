Imports System.IO.Compression

Public Interface ISnoCollection

    ReadOnly Property Database As SnoDatabase
    ReadOnly Property Name As String
    Function LoadAsync(entries As IEnumerable(Of ZipArchiveEntry)) As Task
    Function SaveAsync(archive As ZipArchive) As Task

End Interface

Public Interface ISnoCollection(Of T)
    Inherits ISnoCollection

    Sub AddOrReplace(item As T)
    Sub Remove(item As T)
    Function Query() As IQueryable(Of T)
    Overloads Function GetItemPath(item As T) As String

End Interface