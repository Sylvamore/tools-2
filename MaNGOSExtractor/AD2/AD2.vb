﻿Imports System
Imports System.IO
Imports AD2.MaNGOSExtractorCore

Module AD2

    Sub Main()
        Console.WriteLine(" ")
        Console.WriteLine("MaNGOSExtractor{0} CommandLine", MaNGOSExtractorCore.Version())
        Console.WriteLine("================================")
        Console.WriteLine(" ")
        If My.Application.CommandLineArgs.Count = 0 Then
            Console.WriteLine("Usage:")
            Console.WriteLine("ad2 -[opt] [value]")
            Console.WriteLine("-i set input path")
            Console.WriteLine("-o set output path")
            Console.WriteLine("-e {1/2/3} 1 Maps Only, 2 DBC Only, 3 both DBC and Maps   Default is 3")
            End
        Else
            Dim strExtractionLevel As String = ""
            Dim strInputFolder As String = ""
            Dim strOutputFolder As String = ""
            Dim intMaxCommands As Integer = My.Application.CommandLineArgs.Count - 1
            Dim blnCMDError As Boolean = False
            For Commands As Integer = 0 To intMaxCommands
                Select Case My.Application.CommandLineArgs(Commands)
                    Case "-e"
                        If Commands < intMaxCommands Then
                            strExtractionLevel = My.Application.CommandLineArgs(Commands + 1)
                        Else
                            strExtractionLevel = ""
                        End If

                        Select Case strExtractionLevel
                            Case "1"
                                Console.WriteLine("Extraction Selected: Maps Only")
                            Case "2"
                                Console.WriteLine("Extraction Selected: DBC Only")
                            Case "3"
                                Console.WriteLine("Extraction Selected: Both DBC and Maps")
                            Case Else
                                Console.WriteLine("Extraction Selected: *ERROR* - Invalid Option '" & strExtractionLevel & "'")
                                blnCMDError = True
                        End Select
                        Commands = Commands + 1
                    Case "-i"
                        If Commands < intMaxCommands Then
                            strInputFolder = My.Application.CommandLineArgs(Commands + 1)
                        Else
                            strInputFolder = ""
                        End If

                        If System.IO.Directory.Exists(strInputFolder) = True Then
                            Console.WriteLine("Source Folder: " & strInputFolder)
                        Else
                            Console.WriteLine("Source Folder: *ERROR* - Folder '" & strInputFolder & "' was not found (or accessible)")
                            blnCMDError = True

                        End If
                        Commands = Commands + 1

                    Case "-o"
                        If Commands < intMaxCommands Then
                            strOutputFolder = My.Application.CommandLineArgs(Commands + 1)
                        Else
                            strOutputFolder = ""
                        End If

                        If System.IO.Directory.Exists(strOutputFolder) = True Then
                            Console.WriteLine("Output Folder: " & strOutputFolder)
                        Else
                            Try
                                System.IO.Directory.CreateDirectory(strOutputFolder)
                                Console.WriteLine("Output Folder: " & strOutputFolder)
                            Catch ex As Exception
                                Console.WriteLine("Output Folder: *ERROR* - Folder '" & strOutputFolder & "' could not be created")
                                blnCMDError = True
                            End Try
                        End If
                        Commands = Commands + 1

                    Case Else
                        Console.WriteLine("Command={0}", My.Application.CommandLineArgs(Commands))
                End Select
            Next

            'If any parameters have been flagged as having an error, bail out
            If blnCMDError = True Then End

            'At this stage we should have the options we need plus parameters and paths
            Dim colBaseFiles As New SortedSet(Of String)     'Collection containing all the base files
            Dim colMainFiles As New SortedSet(Of String)    'Collection containing all the main files
            Dim colUpdateFiles As New SortedSet(Of String)   'Collection containing any update or patch files

            Dim colFolders As New Collection                'Collection to hold for the folders to be processed
            Dim myFolders As System.IO.DirectoryInfo

            If System.IO.Directory.Exists(strInputFolder) = False Then
                Console.WriteLine("Warcraft folder '{0}' can not be located", strInputFolder)
                Exit Sub
            End If

            'Set the Top level as {Wow Folder}\data
            myFolders = New System.IO.DirectoryInfo(strInputFolder & "\data")

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

            If strOutputFolder.EndsWith("\") = False Then strOutputFolder = strOutputFolder & "\"
            If My.Computer.FileSystem.DirectoryExists(strOutputFolder) = False Then
                Directory.CreateDirectory(strOutputFolder)
            End If

            For Each strItem As String In colMainFiles
                Console.WriteLine("  BASE: " & strItem)
                Try
                    ExtractDBCFiles(strItem, "*.db*", strOutputFolder)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            For Each strItem As String In colMainFiles
                Console.WriteLine("  FILE: " & strItem)
                Try
                    ExtractDBCFiles(strItem, "*.db*", strOutputFolder)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next

            For Each strItem As String In colUpdateFiles
                Console.WriteLine("UPDATE: " & strItem)

                Try
                    '                    Me.Text = strItem
                    ExtractDBCFiles(strItem, "*.db*", strOutputFolder)
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            Next
            Console.WriteLine("Finished")


            End
        End If
    End Sub

End Module