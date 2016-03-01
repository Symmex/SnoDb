Imports System.IO.Compression

Public Class RemoveSaveAction
    Implements ISaveAction

    Public ReadOnly Property ItemPath As String

    Public Sub New(itemPath As String)
        Me.ItemPath = itemPath
    End Sub

    Public Function SaveAsync(archive As ZipArchive) As Task Implements ISaveAction.SaveAsync
        Dim entry = archive.GetEntry(Me.ItemPath)

        If entry IsNot Nothing Then
            entry.Delete()
        End If

        Return Task.CompletedTask
    End Function

End Class