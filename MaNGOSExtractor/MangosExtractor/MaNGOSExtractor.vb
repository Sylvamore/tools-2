﻿Imports System
Imports System.IO
Imports MangosExtractor.MaNGOSExtractorCore

Public Class MaNGOSExtractor
    ''' <summary>
    ''' Starts the DBC Extraction process
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnStartDBC_Click(sender As Object, e As EventArgs) Handles btnStartDBC.Click
        ListBox1.Items.Clear()
        Dim colBaseFiles As New SortedSet(Of String)    'Collection containing all the base files
        Dim colMainFiles As New SortedSet(Of String)    'Collection containing all the main files
        Dim colUpdateFiles As New SortedSet(Of String)  'Collection containing any update or patch files

        Dim colFolders As New Collection                'Collection to hold for the folders to be processed
        Dim myFolders As System.IO.DirectoryInfo

        If System.IO.Directory.Exists(txtBaseFolder.Text) = False Then
            MessageBox.Show("Warcraft folder '" & txtBaseFolder.Text & "' can not be located")
            Exit Sub
        End If


        'Set the Top level as {Wow Folder}\data
        myFolders = New System.IO.DirectoryInfo(txtBaseFolder.Text & "\data")

        'Add the Data folder to the collection before we start walking down the tree
        colFolders.Add(myFolders, myFolders.FullName)

        'Build a list of all the subfolders under data
        ReadFolders(myFolders, colFolders)

        'Now we need to walk through the folders, getting the MPQ files along the way
        For t As Integer = 1 To colFolders.Count()
            myFolders = colFolders.Item(t)
            For Each file As System.IO.FileInfo In myFolders.GetFiles("*.MPQ")
                If file.FullName.ToLower.Contains("update") = True Or file.FullName.ToLower.Contains("patch") = True Then
                    colUpdateFiles.Add(file.FullName)
                ElseIf file.FullName.ToLower.Contains("base") = True Then
                    colBaseFiles.Add(file.FullName)
                Else
                    colMainFiles.Add(file.FullName)
                End If
            Next
        Next

        If txtOutputFolder.Text.EndsWith("\") = False Then txtOutputFolder.Text = txtOutputFolder.Text & "\"
        If My.Computer.FileSystem.DirectoryExists(txtOutputFolder.Text) = False Then
            Directory.CreateDirectory(txtOutputFolder.Text)
        End If


        For Each strItem As String In colBaseFiles
            ListBox1.Items.Add("  BASE: " & strItem)
            Try
                Me.Text = strItem
                ExtractDBCFiles(strItem, "*.db*", txtOutputFolder.Text)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Next

        For Each strItem As String In colMainFiles
            ListBox1.Items.Add("  FILE: " & strItem)
            Try
                Me.Text = strItem
                ExtractDBCFiles(strItem, "*.db*", txtOutputFolder.Text)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Next

        For Each strItem As String In colUpdateFiles
            ListBox1.Items.Add("UPDATE: " & strItem)

            Try
                Me.Text = strItem
                ExtractDBCFiles(strItem, "*.db*", txtOutputFolder.Text)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Next
        MessageBox.Show("Finished")
    End Sub

    ''' <summary>
    ''' Exits the Application
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BtnQuit_Click(sender As Object, e As EventArgs) Handles BtnQuit.Click
        End
    End Sub

    Private Sub MaNGOSExtractor_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Text = "MaNGOSExtractor" & MaNGOSExtractorCore.Version()
    End Sub
End Class