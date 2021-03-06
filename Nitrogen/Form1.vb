﻿'------------------------------------------------------------------------------
' GN Field - A program to block GameGuard from detecting the PSO2 Tweaker.
' (Internal codename: Nitrogen)
'
' Thanks for taking a look at this code.
' Feel free to submit bugfixes/improvements to 
' https://github.com/Arks-Layer/GNShield
' 
' Take care, and have fun in everything you do. - AIDA
' Program uses the MIT License
'
'------------------------------------------------------------------------------
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Microsoft.Win32

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim Pso2RootDir As String = RegKey.GetValue(Of String)(RegKey.Pso2Dir)
        Dim asm As Assembly = Assembly.GetExecutingAssembly()
            Dim fi As New FileInfo(asm.Location)
            Me.Visible = False
            Me.Hide()
            Me.Opacity = 0
            Me.ShowInTaskbar = False
            Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
            ToolStripTextBox1.Text = "Built: " & fi.LastWriteTime.ToString.Replace("/2016", "/16")
        Hide()
        If File.Exists("gnfieldlog.txt") Then File.Delete("gnfieldlog.txt")
        Log("Checking for renamed Tweakers.")
        Log("Waiting for PSO2 Tweaker to close...")
        NotifyIcon1.ShowBalloonTip(2000, "GN Field", "Waiting for the Tweaker to fully close... (Step 1)", ToolTipIcon.Info)
        Do While Helper.IsFileInUse("PSO2 Tweaker.exe")
            'Probably the worst way to do this ever of all time [AIDA]
            Thread.Sleep(5000)
            If Process.GetProcessesByName("PSO2 Tweaker").Length > 0 Then
                Log("Tweaker took too long, closing Tweaker process!")
                For Each proc As Process In Process.GetProcessesByName("PSO2 Tweaker")
                    proc.Kill()
                Next
            End If
            Application.DoEvents()
        Loop
        If File.Exists("PSO2 Tweaker.donotdetectmegameguardsenpaipls") = True Then
            NotifyIcon1.ShowBalloonTip(2000, "GN Field", "Found a renamed Tweaker, fixing it now!", ToolTipIcon.Info)
            Try
                Log("Renaming a donotdetect Tweaker.")
                File.Copy("PSO2 Tweaker.donotdetectmegameguardsenpaipls", "PSO2 Tweaker.exe", True)
                Thread.Sleep(2000)
                Log("Deleting original donodetect Tweaker.")
                File.Delete("PSO2 Tweaker.donotdetectmegameguardsenpaipls")
            Catch ex As Exception
                MsgBox("Error - " & ex.Message)
            End Try
            End
        End If
        If File.Exists("PSO2 Tweaker.donotnoticemegameguardsenpaipls") = True Then
            NotifyIcon1.ShowBalloonTip(2000, "GN Field", "Found a renamed Tweaker, fixing it now!", ToolTipIcon.Info)
            Try
                Log("Renaming a donotnotice Tweaker.")
                File.Copy("PSO2 Tweaker.donotnoticemegameguardsenpaipls", "PSO2 Tweaker.exe", True)
                Thread.Sleep(2000)
                Log("Deleting original donotnotice Tweaker.")
                File.Delete("PSO2 Tweaker.donotnoticemegameguardsenpaipls")
            Catch ex As Exception
                MsgBox("Error - " & ex.Message)
            End Try
            End
        End If
        If File.Exists("PSO2 Tweaker.exe") = False Then
            MsgBox("GN Field establishment failed! Insufficient GN particles!" & vbCrLf & vbCrLf & "This program should not be run by itself, the PSO2 Tweaker will use it when it needs to. Please make sure that the PSO2 Tweaker is named exactly ""PSO2 Tweaker.exe"".", MessageBoxIcon.Warning)
            End
        End If
        Log("Trying to copy vanilla tweaker to donotnotice")
        File.Copy("PSO2 Tweaker.exe", "PSO2 Tweaker.donotnoticemegameguardsenpaipls", True)
        Log("Deleting original Tweaker")
        File.Delete("PSO2 Tweaker.exe")

        Dim startInfo As ProcessStartInfo = New ProcessStartInfo With {.FileName = (Pso2RootDir & "\pso2.exe"), .Arguments = "+0x33aca2b9", .UseShellExecute = False}
        startInfo.EnvironmentVariables("-pso2") = "+0x01e3f1e9"
        Dim shell As Process = New Process With {.StartInfo = startInfo}

        Log("Starting PSO2 from GN Field!")
        shell.Start()

        Dim hWnd As IntPtr = External.FindWindow("Phantasy Star Online 2", Nothing)
        NotifyIcon1.ShowBalloonTip(2000, "GN Field", "GN Field activated, Tweaker renamed, launching PSO2! (Step 2)", ToolTipIcon.Info)
        tmrWaitingforPSO2.Enabled = True
        Log("Waiting for PSO2 to start")
        Do While hWnd = IntPtr.Zero
            hWnd = External.FindWindow("Phantasy Star Online 2", Nothing)
            Thread.Sleep(10)
            Application.DoEvents()
        Loop
        Log("PSO2 started!")

        Helper.DeleteFile(Pso2RootDir & "\ddraw.dll")
        tmrWaitingforPSO2.Enabled = False
        File.Copy(Pso2RootDir & "\pso2.exe", Pso2RootDir & "\pso2.exe_backup", True)

        Try
            Log("Waiting for PSO2 to close...")
            Do While Helper.IsFileInUse(Pso2RootDir & "\pso2.exe")
                Thread.Sleep(1000)
                'Probably the worst way to do this ever of all time [AIDA]
                Application.DoEvents()
            Loop
            Log("Restoring original pso2.exe")
            File.Copy(Pso2RootDir & "\pso2.exe_backup", Pso2RootDir & "\pso2.exe", True)
            File.Delete(Pso2RootDir & "\pso2.exe_backup")
            Try
                Log("Renaming from donotnotice to vanilla")
                File.Copy("PSO2 Tweaker.donotnoticemegameguardsenpaipls", "PSO2 Tweaker.exe", True)
                NotifyIcon1.ShowBalloonTip(2000, "GN Field", "PSO2 closed, renaming Tweaker and disabling GN Field. (Step 3) Goodbye!", ToolTipIcon.Info)
                Thread.Sleep(2000)
                Log("Deleteing donotnotice Tweaker")
                File.Delete("PSO2 Tweaker.donotnoticemegameguardsenpaipls")
                Close()
            Catch ex As Exception
                MsgBox("Error - " & ex.Message)
            End Try
        Catch ex As Exception
            MsgBox("Error - " & ex.Message)
        End Try
        End
        End
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Dim MsgBoxResultExit As MsgBoxResult = MsgBox("Are you sure you want to exit? This will NOT rename the Tweaker, and you'll have to manually rename it.", vbYesNo)
        NotifyIcon1.ShowBalloonTip(2000, "GN Field", "GN Field is now offline.", ToolTipIcon.Info)
        If MsgBoxResultExit = vbYes Then Close()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Try
            Log("User said they're done playing PSO2, rename it back!")
            File.Copy("PSO2 Tweaker.donotnoticemegameguardsenpaipls", "PSO2 Tweaker.exe", True)
            NotifyIcon1.ShowBalloonTip(2000, "GN Field", "Renaming Tweaker and disabling GN Field. Goodbye!", ToolTipIcon.Info)
            Thread.Sleep(2000)
            Log("Deleting donotnotice Tweaker")
            File.Delete("PSO2 Tweaker.donotnoticemegameguardsenpaipls")
            Close()
        Catch ex As Exception
            MsgBox("Error - " & ex.Message)
        End Try
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        MsgBox("The GN Field is a program meant to hide the PSO2 Tweaker from Gameguard. It does this (currently) by renaming the Tweaker after it's launched, to PSO2 Tweaker.donotnoticemegameguardsenpaipls instead of PSO2 Tweaker.exe. If you find your PSO2 Tweaker doesn't work after closing PSO2, please see if that renamed file is present, and rename it back.")
    End Sub

    Public Sub Log(s As String)
        File.AppendAllText(Application.StartupPath & "\gnfieldlog.txt", DateTime.Now.ToString("G") & ": DEBUG - " & s & vbCrLf)
    End Sub

End Class
Public Class External
    Public Declare Function EnumDisplaySettings Lib "user32" Alias "EnumDisplaySettingsA" (ByVal lpszDeviceName As String, ByVal iModeNum As Integer, ByRef lpDevMode As Devmode) As Boolean

    Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr

    Public Declare Function FlashWindow Lib "user32" (ByVal hwnd As IntPtr, ByVal bInvert As Boolean) As Boolean

    Public Declare Auto Function ShellExecute Lib "shell32" (ByVal hwnd As IntPtr, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As UInteger) As IntPtr

    Public Declare Function ShGetKnownFolderPath Lib "shell32" Alias "SHGetKnownFolderPath" (ByRef id As Guid, flags As Integer, token As IntPtr, ByRef path As IntPtr) As Integer

    <StructLayout(LayoutKind.Sequential)> Public Structure Devmode
        <MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmOrientation As Short
        Public dmPaperSize As Short
        Public dmPaperLength As Short
        Public dmPaperWidth As Short
        Public dmScale As Short
        Public dmCopies As Short
        Public dmDefaultSource As Short
        Public dmPrintQuality As Short
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmFormName As String
        Public dmUnusedPadding As Short
        Public dmBitsPerPel As Short
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
    End Structure
End Class

Public Class RegKey
    Public Const SteamMode = "SteamMode"
    Public Const Pso2Dir = "PSO2Dir"

    Private Shared ReadOnly RegistryCache As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    Private Shared ReadOnly RegistrySubKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\AIDA", True)

    Public Shared Function GetValue(Of T)(key As String) As T
        Try
            Dim returnValue As Object = Nothing
            If RegistryCache.TryGetValue(key, returnValue) Then Return DirectCast(Convert.ChangeType(returnValue, GetType(T)), T)

            returnValue = RegistrySubKey.GetValue(key, Nothing)
            If returnValue IsNot Nothing Then RegistryCache.Add(key, returnValue)

            Return DirectCast(Convert.ChangeType(returnValue, GetType(T)), T)
        Catch
            Return Nothing
        End Try
    End Function
End Class

Public Class Helper
    Private Shared ReadOnly SizeSuffixes As String() = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"}
    Private Shared ReadOnly HexTable As String() = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F", "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF", "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"}
    Private Shared ReadOnly FolderDownloads As New Guid("374DE290-123F-4565-9164-39C4925E467B")
    Private Shared ReadOnly Generator As Random = New Random()

    Public Shared ReadOnly DefaltCultureInfo As CultureInfo = New CultureInfo("en")


    Public Shared Sub DeleteFile(path As String)
        If File.Exists(path) Then File.Delete(path)
    End Sub



    Public Shared Function IsFileInUse(ByVal path As String) As Boolean
        Try
            Using stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                stream.Close()
            End Using
        Catch
            Return True
        End Try

        Return False
    End Function



End Class