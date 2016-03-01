Imports System.IO
Imports System.IO.Compression
Imports Newtonsoft.Json

Public Class AddOrReplaceSaveAction
    Implements ISaveAction

    Public ReadOnly Property ItemPath As String
    Public ReadOnly Property Item As Object

    Public Sub New(itemPath As String, item As Object)
        Me.ItemPath = itemPath
        Me.Item = item
    End Sub

    Public Async Function SaveAsync(archive As ZipArchive) As Task Implements ISaveAction.SaveAsync
        Dim itemJson = JsonConvert.SerializeObject(Item)

        Dim entry = archive.GetEntry(ItemPath)
        If entry IsNot Nothing Then
            entry.Delete()
        End If

        entry = archive.CreateEntry(ItemPath)
        Using writer = New StreamWriter(entry.Open())
            Await writer.WriteAsync(itemJson)
        End Using
    End Function

End Class