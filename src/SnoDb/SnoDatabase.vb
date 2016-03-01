Imports System.IO
Imports System.IO.Compression
Imports System.Collections.Concurrent

Public Class SnoDatabase

    Public ReadOnly Property Name As String
    Public ReadOnly Property DatabaseDirectory As String
    Private ReadOnly Property ArchivePath As String
    Private ReadOnly Property Collections As ConcurrentDictionary(Of String, ISnoCollection) = New ConcurrentDictionary(Of String, ISnoCollection)()

    Public Sub New(name As String)
        Me.New(name, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SnoDb\Databases\"))
    End Sub

    Public Sub New(name As String, databaseDirectory As String)
        Me.Name = name
        Me.DatabaseDirectory = databaseDirectory

        Directory.CreateDirectory(Me.DatabaseDirectory)
        Me.ArchivePath = Path.Combine(Me.DatabaseDirectory, $"{name}.zip")
    End Sub

    Public Function RegisterCollection(Of T, TId)(idSelector As Func(Of T, TId)) As ISnoCollection(Of T)
        Dim collectionName = Me.GetCollectionName(Of T)()
        Dim collection = New SnoCollection(Of T, TId)(Me, collectionName, idSelector)

        Me.Collections.TryAdd(collectionName, collection)

        Return collection
    End Function

    Public Function GetCollection(Of T)() As ISnoCollection(Of T)
        Dim collectionName = Me.GetCollectionName(Of T)()
        Dim collection = Me.Collections(collectionName)

        Return DirectCast(collection, ISnoCollection(Of T))
    End Function

    Public Async Function LoadAsync() As Task
        If Not File.Exists(Me.ArchivePath) Then
            Return
        End If

        Using archive = ZipFile.Open(Me.ArchivePath, ZipArchiveMode.Read)
            Dim entriesByCollection = (From entry In archive.Entries
                                       Let separatorIndex = entry.FullName.IndexOf("/"c)
                                       Let collectionName = entry.FullName.Substring(0, separatorIndex)
                                       Group entry By collectionName Into Group
                                       Select New With {.CollectionName = collectionName, .Entries = Group})

            For Each collectionEntries In entriesByCollection
                Dim currentCollection As ISnoCollection = Nothing
                If Me.Collections.TryGetValue(collectionEntries.CollectionName, currentCollection) Then
                    Await currentCollection.LoadAsync(collectionEntries.Entries)
                End If
            Next
        End Using
    End Function

    Public Async Function SaveAsync() As Task
        Using archive = ZipFile.Open(Me.ArchivePath, ZipArchiveMode.Update)
            For Each collection In Me.Collections.Values
                Await collection.SaveAsync(archive)
            Next
        End Using
    End Function

    Private Function GetCollectionName(Of T)() As String
        Return GetType(T).Name
    End Function

End Class