Imports System.IO
Imports System.IO.Compression
Imports System.Collections.Concurrent
Imports Newtonsoft.Json

Public Class SnoCollection(Of T, TId)
    Implements ISnoCollection(Of T)

    Public ReadOnly Property Database As SnoDatabase Implements ISnoCollection.Database
    Public ReadOnly Property Name As String Implements ISnoCollection.Name
    Private ReadOnly Property IdSelector As Func(Of T, TId)
    Private ReadOnly Property Items As ConcurrentDictionary(Of TId, T) = New ConcurrentDictionary(Of TId, T)()
    Private ReadOnly Property SaveActions As ConcurrentDictionary(Of TId, ISaveAction) = New ConcurrentDictionary(Of TId, ISaveAction)()

    Public Sub New(database As SnoDatabase, name As String, idSelector As Func(Of T, TId))
        Me.Database = database
        Me.Name = name
        Me.IdSelector = idSelector
    End Sub

    Public Sub AddOrReplace(item As T) Implements ISnoCollection(Of T).AddOrReplace
        Dim id = Me.IdSelector(item)
        Me.Items(id) = item

        Dim itemPath = Me.GetItemPath(item)
        Me.SaveActions(id) = New AddOrReplaceSaveAction(itemPath, item)
    End Sub

    Public Sub Remove(item As T) Implements ISnoCollection(Of T).Remove
        Dim id = Me.IdSelector(item)
        Me.Items.TryRemove(id, Nothing)

        Dim itemPath = Me.GetItemPath(item)
        Me.SaveActions(id) = New RemoveSaveAction(itemPath)
    End Sub

    Public Function Query() As IQueryable(Of T) Implements ISnoCollection(Of T).Query
        Return Me.Items.Values.AsQueryable()
    End Function

    Private Function GetItemPath(item As T) As String Implements ISnoCollection(Of T).GetItemPath
        Dim id = Me.IdSelector(item)

        Return $"{Me.Name}/{id}.json"
    End Function

    Public Async Function LoadAsync(entries As IEnumerable(Of ZipArchiveEntry)) As Task Implements ISnoCollection(Of T).LoadAsync
        For Each entry In entries
            Dim itemJson As String
            Using reader = New StreamReader(entry.Open())
                itemJson = Await reader.ReadToEndAsync()
            End Using

            Dim item = JsonConvert.DeserializeObject(Of T)(itemJson)
            Dim id = Me.IdSelector(item)
            Me.Items(id) = item
        Next
    End Function

    Public Async Function SaveAsync(archive As ZipArchive) As Task Implements ISnoCollection.SaveAsync
        For Each saveAction In Me.SaveActions.Values
            Await saveAction.SaveAsync(archive)
        Next
    End Function

End Class