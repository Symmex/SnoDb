Imports System.IO
Imports System.IO.Compression
Imports Newtonsoft.Json

Public Class AddOrReplaceSaveAction
    Implements ISaveAction

    Public ReadOnly Property ItemPath As String
    Public ReadOnly Property ItemJson As String

    Public Sub New(itemPath As String, item As Object)
        Me.ItemPath = itemPath
        Me.ItemJson = JsonConvert.SerializeObject(item)
    End Sub

    Public Async Function SaveAsync(archive As ZipArchive) As Task Implements ISaveAction.SaveAsync
        Dim entry = archive.GetEntry(Me.ItemPath)
        If entry IsNot Nothing Then
            entry.Delete()
        End If

        entry = archive.CreateEntry(Me.ItemPath)
        Using writer = New StreamWriter(entry.Open())
            Await writer.WriteAsync(Me.ItemJson)
        End Using
    End Function

End Class