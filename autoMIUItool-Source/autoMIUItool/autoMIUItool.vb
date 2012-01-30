'\\kjlogic-hv01\e$\GitRepository\autoMIUItool\
Public Class autoMIUItool

    Inherits System.Windows.Forms.Form

    Dim ApplicationPath As String = System.IO.Path.GetDirectoryName( _
        System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
    Dim BaseFolder, InputRoms, OutputRoms, ROMsource, EnglishFolder, LanguageFolder, StockFolder, Theme As String
    Dim DoROMs() As String
    Dim InputAPK, OutputAPK, Temp, Tools, UpdateAPK, UpdateInput, UpdateROM, UpdateZIP, APKjar, DoJava As String
    Dim DoSpecial(), DelAPKs() As String
    Dim FileName, FileNoExtension, FolderName, FlashableSysApp, FlashableSysFrm As String
    Dim UpdaterScript, ResultSTR, apktoolYML As String
    Dim ReportValue As Integer = 0
    Dim checkStringFilename1, checkStringFilename2 As String
    Dim ItemsCount1, ItemsCount2 As Integer
    Private Shared ProcessStdOutData As String = Nothing
    Private Shared ProcessStdErrData As String = Nothing
    Dim DeOdexedRoms, OdexedRoms, DeodexTemp, BakSmalijar, Smalijar As String
    Dim DebugOn As Boolean = False


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            My.Settings.ApplicationPath = ApplicationPath
            My.Settings.BaseFolder = BaseFolder
            My.Settings.Language = Language.Text
            My.Settings.EnglishTranslationFolder = EnglishTranslationFolder.Text
            My.Settings.LanguageTranslationFolder = LanguageTranslationFolder.Text
            My.Settings.StockTranslationFolder = StockTranslationFolder.Text
            My.Settings.APKjarVersion = APKjarVersion.Text
            My.Settings.SmalijarVersion = BakSmaliVersion.Text
            My.Settings.JavaExe = JavaExe.Text
            My.Settings.Template1 = Template1.Text
            My.Settings.SpecialAPKs = SpecialAPK.Text
            My.Settings.DelAPKs = DeleteAPKs.Text
            My.Settings.FTPlocation = FTPlocation.Text
            My.Settings.FTPu = FTPu.Text
            My.Settings.FTPp = FTPp.Text
            My.Settings.Save()
            ExitForm.ShowDialog()
            SaveLog()
        Catch ex As Exception
            MsgBox("There was a problem saving your settings.", _
            MsgBoxStyle.Critical, "Save Error...")
        End Try

    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If My.Settings.BaseFolder <> "" Then
            BaseFolder = My.Settings.BaseFolder
        Else
            SelectBaseFolder()
        End If
        Language.Text = My.Settings.Language
        EnglishTranslationFolder.Text = My.Settings.EnglishTranslationFolder
        LanguageTranslationFolder.Text = My.Settings.LanguageTranslationFolder
        StockTranslationFolder.Text = My.Settings.StockTranslationFolder

        TextBox1.Text = BaseFolder
        InputRoms = BaseFolder & "\_INPUT_ROM\"
        DeOdexedRoms = BaseFolder & "\_DEODEX_ROM\"
        OdexedRoms = BaseFolder & "\_ODEX_ROM\"
        DeodexTemp = BaseFolder & "\_DEODEX_TEMP\"
        OutputRoms = BaseFolder & "\_OUTPUT_ROM\"
        InputAPK = BaseFolder & "\_INPUT_APK\"
        OutputAPK = BaseFolder & "\_OUT_APK\"
        Temp = BaseFolder & "\_TEMP\"
        Theme = BaseFolder & "\_THEME\"
        Tools = BaseFolder & "\_TOOLS\"
        UpdateAPK = BaseFolder & "\_UPDATE_APK\"
        UpdateInput = BaseFolder & "\_UPDATE_INPUT\"
        UpdateROM = BaseFolder & "\_UPDATE_ROM\"
        UpdateZIP = BaseFolder & "\_UPDATE_ZIP\"
        FlashableSysApp = BaseFolder & "\_FLASHABLES\system\app\"
        FlashableSysFrm = BaseFolder & "\_FLASHABLES\system\framework\"

        Logging.Text = Now
        OldVersion.Text = Mid(Format(Date.Today.AddDays(-(CInt((Date.Today.DayOfWeek + 2) Mod 7))), "yy.M.d"), 2)
        NewVersion.Text = Format(Date.Today, "yyyy-MM-dd")
        Template1.Text = My.Settings.Template1
        APKjarVersion.Text = My.Settings.APKjarVersion
        JavaExe.Text = My.Settings.JavaExe
        DoJava = JavaExe.Text
        APKjar = BaseFolder & "\" & APKjarVersion.Text
        DeleteAPKs.Text = My.Settings.DelAPKs
        SpecialAPK.Text = My.Settings.SpecialAPKs
        FTPlocation.Text = My.Settings.FTPlocation
        FTPu.Text = My.Settings.FTPu
        FTPp.Text = My.Settings.FTPp
        BakSmaliVersion.Text = My.Settings.SmalijarVersion
        BakSmalijar = BaseFolder & "\bak" & BakSmaliVersion.Text
        Smalijar = BaseFolder & "\" & BakSmaliVersion.Text

        DoSpecial = Split(SpecialAPK.Text, " ")
        DelAPKs = Split(DeleteAPKs.Text, " ")

        renameInputROMs(InputRoms)
        renameInputROMs(OdexedRoms)

        'Notice how the user settings are writable

        Dim di1 As New IO.DirectoryInfo(InputRoms)
        Dim diarr1 As IO.FileInfo() = di1.GetFiles("*.zip")
        Dim dra1 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra1 In diarr1
            CheckedListBox1.Items.Add(dra1)
        Next
        Dim x As Integer
        For x = 0 To CheckedListBox1.Items.Count - 1
            CheckedListBox1.SetItemChecked(x, True)
        Next x
        For x = 0 To CheckedListBox1.CheckedItems.Count - 1
            ListBox1.Items.Add(CheckedListBox1.CheckedItems(x).ToString)
        Next x

        Dim ddi1 As New IO.DirectoryInfo(OdexedRoms)
        Dim ddiarr1 As IO.FileInfo() = ddi1.GetFiles("*.zip")
        Dim ddra1 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each ddra1 In ddiarr1
            CheckedListBox3.Items.Add(ddra1)
        Next

        CheckedListBox2.Items.Clear()
        Dim di3 As New IO.DirectoryInfo(OutputRoms)
        Dim diarr3 As IO.FileInfo() = di3.GetFiles("*.zip")
        Dim dra3 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra3 In diarr3
            CheckedListBox2.Items.Add(dra3)
        Next

        Dim di2 As New IO.DirectoryInfo(BaseFolder)
        Dim diarr2 As IO.FileInfo() = di2.GetFiles("apktool*.jar")
        Dim dra2 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra2 In diarr2
            APKjarVersion.Items.Add(dra2)
        Next

        Dim ddi2 As New IO.DirectoryInfo(BaseFolder)
        Dim ddiarr2 As IO.FileInfo() = ddi2.GetFiles("smali*.jar")
        Dim ddra2 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each ddra2 In ddiarr2
            BakSmaliVersion.Items.Add(ddra2)
        Next
    End Sub

    Private Sub CleanFolder(ByVal folder As String, Optional ByVal DelFolder As Boolean = False)
        My.Computer.FileSystem.DeleteDirectory(folder, FileIO.DeleteDirectoryOption.DeleteAllContents)
        If Not DelFolder Then System.IO.Directory.CreateDirectory(folder)
    End Sub

    Private Sub renameInputROMs(ByVal folder As String)
        Dim di As New IO.DirectoryInfo(folder)
        Dim foundFiles As IO.FileInfo() = di.GetFiles("*.zip")
        Dim foundFile As IO.FileInfo

        For Each foundFile In foundFiles
            Dim X() As String
            X = Split(foundFile.ToString, "_")
            If X(0) <> "miuinl" Then
                If UCase(X(0)) = "MIUIANDROID" Then
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_{0}", X(1)))
                ElseIf UCase(X(2)) = "D2EXT" Then
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_Desire_{0}-{1}.zip", X(2), X(4)))
                ElseIf UCase(X(2)) = "D2W" Then
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_Desire_{0}-{1}.zip", X(2), X(3)))
                ElseIf UCase(X(2)) = "A2SD" Then
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_Desire_{0}-{1}.zip", X(2), X(3)))
                ElseIf UCase(X(0)) = "OTA" Then
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_Desire_OTA-{0}.zip", X(3)))
                ElseIf UCase(X(0)) = "MIUI" Then 'miui_I9000_2.1.13_5ns5xih80r_2.3
                    My.Computer.FileSystem.RenameFile(folder & foundFile.ToString,
                                                      String.Format("miuinl_{0}-{1}.zip", X(1), X(2)))
                End If
            End If
        Next

    End Sub

    Private Sub TextBox1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.Click
        SelectBaseFolder()
    End Sub

    Private Sub SelectBaseFolder()
        FolderBrowserDialog1.Description = "Select the Base Folder"

        ' Do not show the button for new folder
        FolderBrowserDialog1.ShowNewFolderButton = False

        Dim dlgResult As DialogResult = FolderBrowserDialog1.ShowDialog()

        If dlgResult = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath()
            BaseFolder = TextBox1.Text
        End If

    End Sub


    Private Sub CheckedListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        ListBox1.Items.Clear()
        If CheckedListBox1.CheckedItems.Count <> 0 Then
            ' If so, loop through all checked items and print results.
            Dim x As Integer
            For x = 0 To CheckedListBox1.CheckedItems.Count - 1
                ListBox1.Items.Add(CheckedListBox1.CheckedItems(x).ToString)
            Next x
        End If

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If CheckBox2.Checked Then
            If ListBox1.SelectedItems.Count <> 0 Then SourceROM.Text = ListBox1.SelectedItem.ToString
        Else
            SourceROM.Text = ""
        End If

    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Dim x As Integer
        If CheckBox1.Checked = True Then
            For x = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemChecked(x, True)
            Next x
        Else
            For x = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemChecked(x, False)
            Next x
        End If
        ListBox1.Items.Clear()
        If CheckedListBox1.CheckedItems.Count <> 0 Then
            ' If so, loop through all checked items and print results.
            For x = 0 To CheckedListBox1.CheckedItems.Count - 1
                ListBox1.Items.Add(CheckedListBox1.CheckedItems(x).ToString)
            Next x
        End If

    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If Not CheckBox2.Checked Then
            SourceROM.Text = ""
            GoTranslate.Text = "3. Rebuild All"
        Else
            If ListBox1.SelectedItems.Count <> 0 Then SourceROM.Text = ListBox1.SelectedItem.ToString
            GoTranslate.Text = "1. Decompile"

        End If

    End Sub

    Private Function DOStoUNIX(ByVal DOSstring As String) As String
        Return Replace(DOSstring, vbCrLf, vbLf, 1, Len(DOSstring), vbTextCompare)
    End Function

    Private Function UNIXtoDOS(ByVal UNIXstring As String) As String
        Return Replace(UNIXstring, vbLf, vbCrLf, 1, Len(UNIXstring), vbTextCompare)
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        SaveLog()
        Logging.Text = Now
    End Sub

    Private Sub DoLog(ByVal LogText As String)
        Logging.Text = Logging.Text & vbNewLine & Now & ": " & LogText
    End Sub

    '    Shared Function DoExecute(ByVal process As String, ByVal param As String, ByVal BaseFolder As String) As String
    Shared Function DoExecute(ByVal process As String, ByVal param As String, Optional ByVal WorkDir As String = "") As String
        Dim p As Process = New Process
        ' this is the name of the process we want to execute 
        p.StartInfo.FileName = process
        If WorkDir <> "" Then p.StartInfo.WorkingDirectory = WorkDir
        p.StartInfo.Arguments = param
        ' need to set this to false to redirect output
        p.StartInfo.UseShellExecute = False
        p.StartInfo.RedirectStandardOutput = True
        AddHandler p.OutputDataReceived, _
                   AddressOf NetOutputDataHandler
        ProcessStdOutData = ""
        p.StartInfo.RedirectStandardError = True
        AddHandler p.ErrorDataReceived, _
                   AddressOf NetErrorDataHandler
        ProcessStdErrData = ""
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        ' start the process 
        p.Start()
        ' wait for the process to terminate 
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        p.Close()
        Return ProcessStdOutData & vbNewLine & ProcessStdErrData & vbNewLine
    End Function

    Private Shared Sub NetOutputDataHandler(sendingProcess As Object, _
    outLine As DataReceivedEventArgs)
        ' Collect the net view command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            ' Add the text to the collected output.
            ProcessStdOutData = ProcessStdOutData & vbNewLine & outLine.Data
        End If
    End Sub

    Private Shared Sub NetErrorDataHandler(sendingProcess As Object, _
    outLine As DataReceivedEventArgs)
        ' Collect the net view command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            ' Add the text to the collected output.
            ProcessStdErrData = ProcessStdErrData & vbNewLine & outLine.Data
        End If
    End Sub


    Private Sub EnglishTranslationFolder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EnglishTranslationFolder.Click
        FolderBrowserDialog1.Description = "Select the English Translation Folder"

        ' Do not show the button for new folder
        FolderBrowserDialog1.ShowNewFolderButton = False

        Dim dlgResult As DialogResult = FolderBrowserDialog1.ShowDialog()

        If dlgResult = DialogResult.OK Then
            EnglishTranslationFolder.Text = FolderBrowserDialog1.SelectedPath()
            EnglishFolder = EnglishTranslationFolder.Text
        End If

    End Sub

    Private Sub LanguageTranslationFolder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LanguageTranslationFolder.Click
        FolderBrowserDialog1.Description = "Select the " & Language.Text & " Translation Folder"

        ' Do not show the button for new folder
        FolderBrowserDialog1.ShowNewFolderButton = False

        Dim dlgResult As DialogResult = FolderBrowserDialog1.ShowDialog()

        If dlgResult = DialogResult.OK Then
            LanguageTranslationFolder.Text = FolderBrowserDialog1.SelectedPath()
            LanguageFolder = LanguageTranslationFolder.Text
        End If

    End Sub

    Shared Function GetAllFiles(ByVal folder As String, ByVal filter As String)
        Return My.Computer.FileSystem.GetFiles(folder,
            FileIO.SearchOption.SearchTopLevelOnly, filter)
    End Function

    Shared Function GetAllSubFiles(ByVal folder As String, ByVal filter As String)
        Return My.Computer.FileSystem.GetFiles(folder,
            FileIO.SearchOption.SearchAllSubDirectories, filter)
    End Function

    Shared Function GetAllFolders(ByVal folder As String, ByVal filter As String)
        Return My.Computer.FileSystem.GetDirectories(folder,
            FileIO.SearchOption.SearchTopLevelOnly, filter)
    End Function

    Private Sub xCopy(ByVal Source As String, ByVal Destination As String)
        'MsgBox(Source & "->" & Destination)
        My.Computer.FileSystem.CopyDirectory(Source, Destination, True)
        For Each Directory As String In My.Computer.FileSystem.GetDirectories(Source,
             FileIO.SearchOption.SearchTopLevelOnly, "*")
            xCopy(Directory, Destination & "\" & System.IO.Path.GetFileName(Directory))
        Next
    End Sub

    Private Sub GoTranslate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoTranslate.Click
        Me.GoTranslate.Enabled = False
        ReDim DoROMs(ListBox1.Items.Count - 1)
        ListBox1.Items.CopyTo(DoROMs, 0)
        SaveLog()
        For Each ROM In DoROMs
            DoLog("ROM to translate -> " & ROM)
        Next
        ROMsource = SourceROM.Text
        DoLog("ROM as source: " & ROMsource)

        If ROMsource <> "" Then
            If Not Me.Status1.ForeColor = Color.Lime Then
                StartBackgroundWorker("DecompileAPK")
            End If
            If Not RecompileOK.Checked Then
                If Me.Status1.ForeColor = Color.Lime Then
                    StartBackgroundWorker("RecompileAPK")

                End If
            Else
                StartBackgroundWorker("RebuildAll")
            End If
        Else
            StartBackgroundWorker("RebuildAll")

        End If
    End Sub

    Private Sub StartBackgroundWorker(ByVal what As String)
        Try
            BackgroundWorker1.RunWorkerAsync(what)
        Catch ex As Exception
            MsgBox("Background Thread already running...", MsgBoxStyle.OkOnly, "Not allowed")
        End Try

    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If e.Argument.ToString = "DecompileAPK" Then
            Me.TTstatus.Text = "Decompile APK"
            automatic_part1()
            Me.Status1.ForeColor = Color.Lime
            'MsgBox("To Do: Check Arrays and Strings...")
            Me.GoTranslate.Text = "2. Recompile"
        End If
        If e.Argument.ToString = "RecompileAPK" Then
            Me.TTstatus.Text = "Recompile APK"
            automatic_part2()
            'MsgBox("To Do: Check if NO ERRORs in Recompile")
        End If
        If e.Argument.ToString = "RebuildAll" Then
            Me.TTstatus.Text = "Rebuild APK"
            Me.status3.ForeColor = Color.OrangeRed
            Me.status4.ForeColor = Color.OrangeRed
            Me.status5.ForeColor = Color.OrangeRed
            automatic_part3()
            Me.status3.ForeColor = Color.Lime
            Me.TTstatus.Text = "Rebuild ROM"
            automatic_part4()
            Me.status4.ForeColor = Color.Lime
            Me.TTstatus.Text = "Rebuild ZIP"
            automatic_part5()
            Me.status5.ForeColor = Color.Lime
            If autoUpload.Checked Then doSelectFTP(True)
        End If
        If e.Argument.ToString = "CreateUpdate" Then
            CreateUpdate()
        End If
        If e.Argument.ToString = "CheckArrays" Then
            chkArrays()
        End If
        If e.Argument.ToString = "CheckStringsICS" Then
            chkStringsICS()
        End If
        If e.Argument.ToString = "SortStrings" Then
            srtStrings()
        End If
        If e.Argument.ToString = "MergeStock" Then
            MergeStockStrings()
        End If
        If e.Argument.ToString = "DeOdex" Then
            deodex()
        End If
        If e.Argument.ToString = "Theme" Then
            CreateTheme()
        End If
        If e.Argument.ToString = "FTP" Then
            doSelectFTP(False)
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Me.GoTranslate.Enabled = True
        Me.findStrings2.Enabled = True
        Me.findArrays.Enabled = True
        Me.sortStrings.Enabled = True
        Me.DoDeOdex.Enabled = True
        Me.MakeTheme.Enabled = True
        Me.MergeStock.Enabled = True
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ToolStripProgressBar1.Value = e.ProgressPercentage
        If Mid(e.UserState.ToString, 1, 2) = "@@" Then
            TTstatus.Text = Mid(e.UserState.ToString, 3)
        Else
            If DebugOn Then
                DoLog(e.UserState.ToString)
            Else
                If Mid(e.UserState.ToString, 1, 2) = "##" Then DoLog(Mid(e.UserState.ToString, 3))
            End If
        End If
    End Sub

    Public Function FileExists(ByVal FileFullPath As String) As Boolean
        Dim f As New IO.FileInfo(FileFullPath)
        Return f.Exists
    End Function

    Public Function FolderExists(ByVal FolderPath As String) As Boolean
        Dim f As New IO.DirectoryInfo(FolderPath)
        Return f.Exists
    End Function

    Private Sub ReportProgress(ByVal value As Integer, ByVal message As String)
        For Each line In Split(message, vbNewLine)
            BackgroundWorker1.ReportProgress(value, line)
        Next
    End Sub

    Private Sub automatic_part1() 'Decompile

        Timer1.Start()
        CleanFolder(BaseFolder & "\_FLASHABLES\system\app")
        CleanFolder(BaseFolder & "\_FLASHABLES\system\framework")
        CleanFolder(BaseFolder & "\_OUT_APK")
        CleanFolder(BaseFolder & "\_INPUT_APK")
        CleanFolder(BaseFolder & "\_TEMP")
        BackgroundWorker1.ReportProgress(0, "@@Cleaned all folders")
        BackgroundWorker1.ReportProgress(0, String.Format("##{0}\7za.exe e {1}{2} -o{3} *.apk -r -y", BaseFolder, InputRoms, SourceROM.Text, InputAPK))
        BackgroundWorker1.ReportProgress(0, DoExecute(BaseFolder & "\7za.exe", String.Format(" e {1}{2} -o{3} *.apk -r -y", BaseFolder, InputRoms, SourceROM.Text, InputAPK)))
        BackgroundWorker1.ReportProgress(0, "@@Extracted *.apk from " & SourceROM.Text)
        BackgroundWorker1.ReportProgress(0, String.Format("##{0} -jar {1} if {2}framework-res.apk", DoJava, APKjar, InputAPK))
        BackgroundWorker1.ReportProgress(0, DoExecute(DoJava, String.Format(" -jar {1} if {2}framework-res.apk", DoJava, APKjar, InputAPK), BaseFolder))
        If FileExists(InputAPK & "framework-miui-res.apk") Then
            BackgroundWorker1.ReportProgress(0, String.Format("##{0} -jar {1} if {2}framework-miui-res.apk", DoJava, APKjar, InputAPK))
            BackgroundWorker1.ReportProgress(0, DoExecute(DoJava, String.Format(" -jar {1} if {2}framework-miui-res.apk", DoJava, APKjar, InputAPK), BaseFolder))
        End If
        If FileExists(BaseFolder & "\_FRAMEWORK\com.htc.resources.apk") Then
            BackgroundWorker1.ReportProgress(0, String.Format("##{0} -jar {1} if {2}\_FRAMEWORK\com.htc.resources.apk", DoJava, APKjar, BaseFolder))
            BackgroundWorker1.ReportProgress(0, DoExecute(DoJava, String.Format(" -jar {1} if {2}\_FRAMEWORK\com.htc.resources.apk", DoJava, APKjar, BaseFolder), BaseFolder))
        End If
        ShowProgress(countItems(GetAllFiles(InputAPK, "*.apk")))
        For Each file In GetAllFiles(InputAPK, "*.apk")
            FileName = System.IO.Path.GetFileName(file)
            FileNoExtension = System.IO.Path.GetFileNameWithoutExtension(file)
            BackgroundWorker1.ReportProgress(ShowProgress(), LanguageTranslationFolder.Text & "\" & FileNoExtension)
            If Not My.Computer.FileSystem.DirectoryExists(LanguageTranslationFolder.Text & "\" & FileNoExtension) Then
                My.Computer.FileSystem.DeleteFile(file)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Delete: " & file)
            End If
        Next
        ShowProgress(countItems(GetAllFiles(InputAPK, "*.apk")))
        For Each file In GetAllFiles(InputAPK, "*.apk")
            FileName = System.IO.Path.GetFileName(file)
            FileNoExtension = System.IO.Path.GetFileNameWithoutExtension(file)
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Decompile: " & FileName)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), String.Format("##{0} -jar {1} d -f {2}{3} {2}{4}", DoJava, APKjar, InputAPK, FileName, FileNoExtension))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {1} d -f {2}{3} {2}{4}", DoJava, APKjar, InputAPK, FileName, FileNoExtension), BaseFolder))
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Decompile: Ready")
    End Sub

    Private Sub automatic_part2() 'ReCompile
        ShowProgress(countItems(GetAllFolders(InputAPK, "*")))
        For Each folder In GetAllFolders(InputAPK, "*")
            FolderName = System.IO.Path.GetFileName(folder)
            BackgroundWorker1.ReportProgress(ShowProgress(), String.Format("##Copy Translation files in apk folder: {0}\{1} -> {2}{1}", LanguageTranslationFolder.Text, FolderName, InputAPK))
            xCopy(LanguageTranslationFolder.Text & "\" & FolderName, InputAPK & FolderName)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@ReCompile: " & FolderName & ".apk")

            If FolderName = "framework-miui-res" Then
                If FileExists(BaseFolder & "\_FRAMEWORK\com.htc.resources.apk") Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), String.Format("##{0} -jar {1} if {2}\_FRAMEWORK\com.htc.resources.apk", DoJava, APKjar, BaseFolder))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {1} if {2}\_FRAMEWORK\com.htc.resources.apk", DoJava, APKjar, BaseFolder), BaseFolder))
                End If
                apktoolYML = UNIXtoDOS(System.IO.File.ReadAllText(InputAPK & FolderName & "\apktool.yml"))
                System.IO.File.WriteAllText(InputAPK & FolderName & "\apktool.yml", DOStoUNIX(Replace(apktoolYML, "  - 1", "  - 1" & vbNewLine & "  - 2")))
            End If

            BackgroundWorker1.ReportProgress(ShowProgress(-2), String.Format("##ReCompile: {0} -jar {1} b -f {2}{3}", DoJava, APKjar, InputAPK, FolderName))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {1} b -f {2}{3}", DoJava, APKjar, InputAPK, FolderName), BaseFolder))
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Recompile: Ready")
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Recompile: Ready")
    End Sub

    Private Sub automatic_part3()
        CleanFolder(OutputAPK)
        DoSpecial = Split(SpecialAPK.Text, " ")
        For Each file In DoSpecial
            If FileExists(InputAPK & file & ".apk") Then My.Computer.FileSystem.DeleteFile(InputAPK & file & ".apk")
            If FolderExists(InputAPK & file) Then CleanFolder(InputAPK & file, True)
        Next
        ShowProgress(countItems(GetAllFiles(InputAPK, "*.apk")))
        For Each file In GetAllFiles(InputAPK, "*.apk")
            FileName = System.IO.Path.GetFileName(file)
            FileNoExtension = System.IO.Path.GetFileNameWithoutExtension(file)
            If FileExists(OutputAPK & FileName) Then My.Computer.FileSystem.DeleteFile(OutputAPK & FileName)
            If FileExists(FlashableSysApp & FileName) Then My.Computer.FileSystem.DeleteFile(FlashableSysApp & FileName)
            If FileExists(FlashableSysFrm & FileName) Then My.Computer.FileSystem.DeleteFile(FlashableSysFrm & FileName)
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Rebuilding apk: " & FileName)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Rebuilding apk: " & FileName)
            My.Computer.FileSystem.CopyFile(InputAPK & FileName, OutputAPK & FileName, True)
            If FileExists(InputAPK & FileNoExtension & "\build\apk\resources.arsc") Then
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" a -tzip {0}{1} {2}{3}\build\apk\resources.arsc -mx9", OutputAPK, FileName, InputAPK, FileNoExtension)))
            End If
            If FileExists(InputAPK & FileNoExtension & "\build\apk\classes.dex") Then
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" a -tzip {0}{1} {2}{3}\build\apk\classes.dex -mx9", OutputAPK, FileName, InputAPK, FileNoExtension)))
            End If
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" u -tzip {0}{1} {2}{3}\build\apk\res -uy0 -mx9", OutputAPK, FileName, InputAPK, FileNoExtension)))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" x {0}{1} -o{0}{2}\_TEMP", InputAPK, FileName, FileNoExtension)))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" u -tzip {0}{1} {2}{3}\_TEMP\res -mx9", OutputAPK, FileName, InputAPK, FileNoExtension)))
            CleanFolder(InputAPK & FileNoExtension & "\_TEMP", True)
            My.Computer.FileSystem.CopyFile(OutputAPK & FileName, BaseFolder & "\_FLASHABLES\system\app\" & FileName, True)
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Rebuild apk: Ready")
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Rebuild apk: Ready")
    End Sub

    Private Sub automatic_part4()
        ShowProgress(ListBox1.Items.Count)
        For Each ROM In ListBox1.Items 'GetAllFiles(InputRoms, "*.zip")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Processing ROM: " & ROM)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Processing ROM: " & ROM)
            Dim ROMname As String = System.IO.Path.GetFileName(ROM)
            Dim ROMnameNoExtension As String = System.IO.Path.GetFileNameWithoutExtension(ROM)

            If FileExists(InputAPK & "framework-res.apk") Then My.Computer.FileSystem.DeleteFile(InputAPK & "framework-res.apk")
            If FileExists(InputAPK & "framework-miui-res.apk") Then My.Computer.FileSystem.DeleteFile(InputAPK & "framework-miui-res.apk")
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" e {0}{1} -o{2} framework-res.apk -r -y", InputRoms, ROMname, InputAPK)))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" e {0}{1} -o{2} framework-miui-res.apk -r -y", InputRoms, ROMname, InputAPK)))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {0} if {1}framework-res.apk", APKjar, InputAPK), BaseFolder))
            If FileExists(InputAPK & "framework-miui-res.apk") Then
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {0} if {1}framework-miui-res.apk", APKjar, InputAPK), BaseFolder))
            End If
            DoSpecial = Split(SpecialAPK.Text, " ")
            For Each file In DoSpecial
                FileName = file & ".apk"
                FileNoExtension = file
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ROM: " & ROM & ": " & FileName)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Processing ROM: " & ROM & ": " & FileName)
                If FileExists(OutputAPK & FileName) Then My.Computer.FileSystem.DeleteFile(OutputAPK & FileName)
                If FileExists(FlashableSysApp & FileName) Then My.Computer.FileSystem.DeleteFile(FlashableSysApp & FileName)
                If FileExists(FlashableSysFrm & FileName) Then My.Computer.FileSystem.DeleteFile(FlashableSysFrm & FileName)
                If FileExists(InputAPK & FileName) Then My.Computer.FileSystem.DeleteFile(InputAPK & FileName)
                If FolderExists(InputAPK & file) Then My.Computer.FileSystem.DeleteDirectory(InputAPK & file,
                    FileIO.DeleteDirectoryOption.DeleteAllContents)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), String.Format("##{0}\7za.exe e {1}{2} -o{3} {4} -r -y", BaseFolder, InputRoms, ROMname, InputAPK, FileName))
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", String.Format(" e {1}{2} -o{3} {4} -r -y", BaseFolder, InputRoms, ROMname, InputAPK, FileName)))
                If FileExists(InputAPK & FileName) Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), String.Format("##{0} -jar {1} d -f {2}{3} {2}{4}", DoJava, APKjar, InputAPK, FileName, file))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, String.Format(" -jar {1} d -f {2}{3} {2}{4}", DoJava, APKjar, InputAPK, FileName, file), BaseFolder))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), LanguageTranslationFolder.Text & "\" & file & " -> " & InputAPK & file)
                    xCopy(LanguageTranslationFolder.Text & "\" & file, InputAPK & file)
                    If InStr(ROMname, "miuinl_Desire_") <> 0 Then
                        If FolderExists(LanguageTranslationFolder.Text & "\" & file & ".DIFF") Then
                            BackgroundWorker1.ReportProgress(ShowProgress(-2), LanguageTranslationFolder.Text & "\" & file & ".DIFF -> " & InputAPK & file)
                            xCopy(LanguageTranslationFolder.Text & "\" & file & ".DIFF", InputAPK & file)
                        End If
                    End If
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-zh*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "drawable-zh*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "raw-zh*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-mcc*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-en-rAU*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-en-rCA*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-en-rGB*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-en-rIE*")
                        CleanFolder(folder, True)
                    Next
                    For Each folder In GetAllFolders(InputAPK & "framework-res\res", "values-en-rNZ*")
                        CleanFolder(folder, True)
                    Next

                    If File = "framework-miui-res" Then
                        apktoolYML = UNIXtoDOS(System.IO.File.ReadAllText(InputAPK & File & "\apktool.yml"))
                        System.IO.File.WriteAllText(InputAPK & File & "\apktool.yml", DOStoUNIX(Replace(apktoolYML, "  - 1", "  - 1" & vbNewLine & "  - 2")))
                    End If

                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -jar " & APKjar & " b -f " & InputAPK & file, BaseFolder))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "Rebuilding apk: " & file)
                    My.Computer.FileSystem.CopyFile(InputAPK & FileName, OutputAPK & FileName, True)
                    If FileExists(InputAPK & FileNoExtension & "\build\apk\resources.arsc") Then
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & OutputAPK & FileName & " " & InputAPK & FileNoExtension & "\build\apk\resources.arsc -mx9"))
                    End If
                    If FileExists(InputAPK & FileNoExtension & "\build\apk\classes.dex") Then
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & OutputAPK & FileName & " " & InputAPK & FileNoExtension & "\build\apk\classes.dex -mx9"))
                    End If
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " u -tzip " & OutputAPK & FileName & " " & InputAPK & FileNoExtension & "\build\apk\res -uy0 -mx9"))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " x " & InputAPK & FileName & " -o" & InputAPK & FileNoExtension & "\_TEMP"))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " u -tzip " & OutputAPK & FileName & " " & InputAPK & FileNoExtension & "\_TEMP\res -mx9"))
                    CleanFolder(InputAPK & FileNoExtension & "\_TEMP", True)
                    If file = "framework-res" Or file = "framework-miui-res" Then
                        My.Computer.FileSystem.CopyFile(OutputAPK & FileName, BaseFolder & "\_FLASHABLES\system\framework\" & FileName, True)
                    Else
                        My.Computer.FileSystem.CopyFile(OutputAPK & FileName, BaseFolder & "\_FLASHABLES\system\app\" & FileName, True)
                    End If
                End If

            Next
            If Not FolderExists(Temp & ROMnameNoExtension) Then System.IO.Directory.CreateDirectory(Temp & ROMnameNoExtension)
            xCopy(BaseFolder & "\_FLASHABLES\system", Temp & ROMnameNoExtension & "\system")
            If countItems(GetAllFiles(BaseFolder & "\_APP_REPLACE\", "*.apk")) <> 0 Then
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "Rebuild ROM: Extra files added!")
                xCopy(BaseFolder & "\_APP_REPLACE", Temp & ROMnameNoExtension & "\system\app")
            End If
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Rebuild ROM: Ready")
    End Sub

    Private Sub automatic_part5()
        ShowProgress(ListBox1.Items.Count)
        For Each ROM In ListBox1.Items 'GetAllFiles(InputRoms, "*.zip")
            Dim ROMname As String = System.IO.Path.GetFileName(ROM)
            Dim ROMnameNoExtension As String = System.IO.Path.GetFileNameWithoutExtension(ROM)
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Processing ZIP: " & ROM)

            If FileExists(Temp & ROMnameNoExtension & "\system\framework\framework-res.apk") Then
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "Copying " & InputRoms & ROM & " to " & Temp & ROM)
                My.Computer.FileSystem.CopyFile(InputRoms & ROM, Temp & ROM, True)
                For Each DelAPK In DelAPKs
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " d " & Temp & ROM & " " & DelAPK & " -r"))
                Next
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ZIP: " & ROM & ": replace updater-script")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "##" & BaseFolder & "\7za.exe" & " e " & Temp & ROM & " -o" & Temp & ROMnameNoExtension & "\META-INF\com\google\android updater-script -r -y")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & Temp & ROM & " -o" & Temp & ROMnameNoExtension & "\META-INF\com\google\android updater-script -r -y"))
                UpdaterScript = Template1.Text & UNIXtoDOS(System.IO.File.ReadAllText(Temp & ROMnameNoExtension & "\META-INF\com\google\android\updater-script"))
                System.IO.File.WriteAllText(Temp & ROMnameNoExtension & "\META-INF\com\google\android\updater-script", DOStoUNIX(UpdaterScript))
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ZIP: " & ROM & ": update")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & Temp & ROMname & " " & Temp & ROMnameNoExtension & "\* -mx9"))
                If InStr(ROMname, "_Desire_OTA") <> 0 Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " d " & Temp & ROMname & " media -r"))
                End If
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoJava & " -jar " & BaseFolder & "\_SIGN_ZIP\signapk.jar " & BaseFolder & "\_SIGN_ZIP\testkey.x509.pem " & BaseFolder & "\_SIGN_ZIP\testkey.pk8 " & Temp & ROMname & " " & OutputRoms & ROMnameNoExtension & "_Signed.zip")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ZIP: " & ROM & ": sign")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -jar " & BaseFolder & "\_SIGN_ZIP\signapk.jar " & BaseFolder & "\_SIGN_ZIP\testkey.x509.pem " & BaseFolder & "\_SIGN_ZIP\testkey.pk8 " & Temp & ROMname & " " & OutputRoms & ROMnameNoExtension & "_Signed.zip", BaseFolder))
            End If
            DoSpecial = Split(SpecialAPK.Text, " ")
            For Each file In DoSpecial
                If FileExists(OutputAPK & file & ".apk") Then My.Computer.FileSystem.DeleteFile(OutputAPK & file & ".apk")
                If FileExists(FlashableSysApp & file & ".apk") Then My.Computer.FileSystem.DeleteFile(FlashableSysApp & file & ".apk")
                If FileExists(FlashableSysFrm & file & ".apk") Then My.Computer.FileSystem.DeleteFile(FlashableSysFrm & file & ".apk")
                If FileExists(InputAPK & file & ".apk") Then My.Computer.FileSystem.DeleteFile(InputAPK & file & ".apk")
                If FolderExists(InputAPK & file) Then My.Computer.FileSystem.DeleteDirectory(InputAPK & file,
                    FileIO.DeleteDirectoryOption.DeleteAllContents)
            Next
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Rebuild ZIP: Ready")
    End Sub

    Private Sub APKjarVersion_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles APKjarVersion.SelectedIndexChanged
        APKjar = BaseFolder & "\" & APKjarVersion.Text
        DoLog("APKjar: " & APKjar)
    End Sub

    Private Sub JavaExe_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles JavaExe.Click
        OpenFileDialog1.Title = "Please Select java.exe location"
        OpenFileDialog1.InitialDirectory = "C:\"
        OpenFileDialog1.Filter = "java.exe|java.exe"
        OpenFileDialog1.ShowDialog()
        Dim dlgResult As DialogResult = OpenFileDialog1.ShowDialog()

        If dlgResult = DialogResult.OK Then
            JavaExe.Text = OpenFileDialog1.FileName
        End If

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Application.DoEvents()

    End Sub

    Private Sub RecompileOK_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecompileOK.CheckedChanged
        If RecompileOK.Checked Then
            status2.ForeColor = Color.Lime
            Me.GoTranslate.Text = "3. Rebuild ALL"
        Else
            status2.ForeColor = Color.OrangeRed
            Me.GoTranslate.Text = "2. Recompile"
        End If
    End Sub


    Private Sub chkStringsICS()
        ShowProgress(countItems(GetAllFolders(EnglishTranslationFolder.Text, "*")))
        Dim childForm As checkStringsICS
        For Each folder In GetAllFolders(EnglishTranslationFolder.Text, "*")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@" & folder)
            Dim FolderName As String = System.IO.Path.GetFileName(folder)
            If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml") Then
                If FileExists(EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en\strings.xml") Then
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en\strings.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                Else
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values\strings.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                End If
                If Not stringsMatch(Me.checkStringFilename1, Me.checkStringFilename2) Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-NoMatch: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                    childForm = New checkStringsICS(Me.checkStringFilename1, Me.checkStringFilename2)
                    childForm.ShowDialog()
                End If
            End If
            If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml") Then
                If FileExists(EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en-rGB\strings.xml") Then
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en-rGB\strings.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                Else
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values\strings.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                End If
                If Not stringsMatch(Me.checkStringFilename1, Me.checkStringFilename2) Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-NoMatch: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                    childForm = New checkStringsICS(Me.checkStringFilename1, Me.checkStringFilename2)
                    childForm.ShowDialog()
                End If
            End If
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(), "@@check Strings ready")

    End Sub

    Public Function stringsMatch(ByVal CFS1 As String, ByVal CFS2 As String) As Boolean
        Dim dictDoc1 As New Dictionary(Of String, String)
        Dim dictDoc2 As New Dictionary(Of String, String)
        Dim dictDoc3 As New Dictionary(Of String, String)
        Dim str, keyName As String
        Dim keyNameStart, keyNameEnd As Integer
        Dim NoNewXMLkeyAddition As Boolean = True
        Dim NoKeysToKeep As Boolean = False

        Dim objReader1 As New System.IO.StreamReader(CFS1, System.Text.Encoding.UTF8)
        Do While objReader1.Peek() <> -1
            str = Trim(objReader1.ReadLine())
            If InStr(str, "<string name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                If InStr(str, " />") <> 0 Then
                    dictDoc1.Add(keyName, 0)
                ElseIf InStr(str, "</string>") <> 0 Then
                    dictDoc1.Add(keyName, 1)
                Else
                    dictDoc1.Add(keyName, 2)
                End If
                'keys in current ROM
                'BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-NoMatch-Key1: " & keyName & "=" & dictDoc1.Item(keyName))
            End If
        Loop
        objReader1.Close()

        Dim objReader2 As New System.IO.StreamReader(CFS2, System.Text.Encoding.UTF8)
        Do While objReader2.Peek() <> -1
            str = Trim(objReader2.ReadLine())
            If InStr(str, "<string name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                If dictDoc2.ContainsKey(keyName) Then dictDoc2.Remove(keyName)
                If dictDoc1.ContainsKey(keyName) Then
                    If InStr(str, " />") <> 0 Then
                        dictDoc2.Add(keyName, 0)
                    ElseIf InStr(str, "</string>") <> 0 Then
                        dictDoc2.Add(keyName, 1)
                    Else
                        dictDoc2.Add(keyName, 2)
                    End If
                    'BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-NoMatch-Key2: " & keyName & "=" & dictDoc2.Item(keyName))
                Else
                    NoNewXMLkeyAddition = False
                End If
            End If
        Loop
        objReader2.Close()

        Dim keys1 As List(Of String) = dictDoc1.Keys.ToList
        keys1.Sort()

        'textbox1 = new to translate
        For Each key1 In keys1
            If Not dictDoc2.ContainsKey(key1) Then
                NoNewXMLkeyAddition = False
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-New-Key: " & key1)
            Else
                If dictDoc1.Item(key1) <> dictDoc2.Item(key1) Then
                    NoNewXMLkeyAddition = False
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkStrings-NewType-Key: " & key1)
                End If
            End If
        Next

        If NoNewXMLkeyAddition Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function arraysMatch(ByVal CFS1 As String, ByVal CFS2 As String) As Boolean
        Dim dictDoc1 As New Dictionary(Of String, Integer)
        Dim dictDoc2 As New Dictionary(Of String, Integer)
        Dim dictDoc11 As New Dictionary(Of String, String)
        Dim dictDoc21 As New Dictionary(Of String, String)
        Dim str, keyName As String
        Dim keyNameStart, keyNameEnd As Integer
        Dim NewStringsCount As Integer = 0
        Dim OldStringsCount As Integer = 0
        Dim NoNewXMLkeyAddition As Boolean = True
        Dim NoKeysToKeep As Boolean = False
        Dim multiLine As Boolean = False
        Dim objReader11 As New ArrayList
        Dim objReader12 As New ArrayList
        Dim objReader21 As New ArrayList
        Dim objReader22 As New ArrayList
        Dim itemCount1 As Integer
        Dim itemCount2 As Integer
        Dim objReader1 As New System.IO.StreamReader(CFS1, System.Text.Encoding.UTF8)
        multiLine = False
        Do While objReader1.Peek() <> -1
            str = objReader1.ReadLine()
            If InStr(str, "array name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                itemCount1 = 1
                objReader12.Add(str)
                multiLine = True
                If InStr(str, " />") <> 0 Then
                    multiLine = False
                    dictDoc1.Add(keyName, itemCount1)
                    dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                    objReader12.Clear()
                End If
            Else
                If multiLine Then
                    objReader12.Add(str)
                    itemCount1 += 1
                    If InStr(str, "array>") <> 0 Then
                        multiLine = False
                        dictDoc1.Add(keyName, itemCount1)
                        dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    End If
                End If
            End If
        Loop
        objReader1.Close()

        Dim objReader2 As New System.IO.StreamReader(CFS2, System.Text.Encoding.UTF8)
        multiLine = False
        Do While objReader2.Peek() <> -1
            str = objReader2.ReadLine()
            If InStr(str, "array name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                If dictDoc2.ContainsKey(keyName) Then dictDoc2.Remove(keyName)
                If dictDoc21.ContainsKey(keyName) Then dictDoc21.Remove(keyName)
                If dictDoc1.ContainsKey(keyName) Then
                    itemCount2 = 1
                    objReader22.Add(str)
                    multiLine = True
                    If InStr(str, " />") <> 0 Then
                        multiLine = False
                        dictDoc2.Add(keyName, itemCount2)
                        dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                        objReader22.Clear()
                    End If
                Else
                    NoNewXMLkeyAddition = False
                End If
            Else
                If multiLine Then
                    objReader22.Add(str)
                    itemCount2 += 1
                    If InStr(str, "array>") <> 0 Then
                        multiLine = False
                        dictDoc2.Add(keyName, itemCount2)
                        dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                        objReader22.Clear()
                    End If
                End If
            End If
        Loop
        objReader2.Close()

        Dim keys11 As List(Of String) = dictDoc11.Keys.ToList
        keys11.Sort()

        'textbox1 = new to translate
        For Each key11 In keys11
            If Not dictDoc2.ContainsKey(key11) Then
                NoNewXMLkeyAddition = False
                NewStringsCount += 1
            Else
                If dictDoc1.Item(key11) <> dictDoc2.Item(key11) Then
                    NoNewXMLkeyAddition = False
                    NewStringsCount += 1
                Else
                End If
            End If
        Next
        Return NoNewXMLkeyAddition
    End Function

    Private Sub findStrings2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles findStrings2.Click
        Me.findStrings2.Enabled = False
        StartBackgroundWorker("CheckStringsICS")
    End Sub

    Private Sub chkArrays()
        Dim childForm As checkArrays
        ShowProgress(countItems(GetAllFolders(EnglishTranslationFolder.Text, "*")))
        For Each folder In GetAllFolders(EnglishTranslationFolder.Text, "*")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@" & folder)
            Dim FolderName As String = System.IO.Path.GetFileName(folder)
            If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\arrays.xml") Then
                If FileExists(EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en\arrays.xml") Then
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en\arrays.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\arrays.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                Else
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values\arrays.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\arrays.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                End If
                'Dim c1 As List(Of String) = System.IO.File.ReadAllLines(checkStringFilename1).ToList
                'Dim c2 As List(Of String) = System.IO.File.ReadAllLines(checkStringFilename2).ToList
                If Not arraysMatch(Me.checkStringFilename1, Me.checkStringFilename2) Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays-NoMatch: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                    childForm = New checkArrays(Me.checkStringFilename1, Me.checkStringFilename2)
                    childForm.ShowDialog()
                End If
                'If c1.Count <> c2.Count Then
                '    childForm = New checkArrays(Me.checkStringFilename1, Me.checkStringFilename2)
                '    childForm.ShowDialog()
                'End If
            End If
            If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\arrays.xml") Then
                If FileExists(EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en-rGB\arrays.xml") Then
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values-en\arrays.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\arrays.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                Else
                    checkStringFilename1 = EnglishTranslationFolder.Text & "\" & FolderName & "\res\values\arrays.xml"
                    checkStringFilename2 = LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\arrays.xml"
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                End If
                'Dim c1 As List(Of String) = System.IO.File.ReadAllLines(checkStringFilename1).ToList
                'Dim c2 As List(Of String) = System.IO.File.ReadAllLines(checkStringFilename2).ToList
                If Not arraysMatch(Me.checkStringFilename1, Me.checkStringFilename2) Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), "checkArrays-NoMatch: " & checkStringFilename1 & " <-> " & checkStringFilename2)
                    childForm = New checkArrays(Me.checkStringFilename1, Me.checkStringFilename2)
                    childForm.ShowDialog()
                End If
                'If c1.Count <> c2.Count Then
                '    childForm = New checkArrays(Me.checkStringFilename1, Me.checkStringFilename2)
                '    childForm.ShowDialog()
                'End If
            End If
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(), "@@check Arrays ready")

    End Sub

    Private Sub findArrays_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles findArrays.Click
        Me.findArrays.Enabled = False
        StartBackgroundWorker("CheckArrays")
    End Sub

    Private Sub SaveLog()
        Dim LogName As String = BaseFolder & "\" & Format(Now, "yyyy-MM-dd-HH-mm-ss") & ".log"
        System.IO.File.WriteAllText(LogName, Logging.Text, System.Text.Encoding.UTF8)
        Logging.Text = Now

    End Sub

    Public Function countItems(ByVal items As Object) As Integer
        Return items.Count
    End Function

    Public Function ShowProgress(Optional ByVal items As Integer = -1) As Integer
        If items = -2 Then
            Return ItemsCount1 * ItemsCount2
            Exit Function
        End If
        If items = -1 Then
            ItemsCount2 += 1
        Else
            BackgroundWorker1.ReportProgress(0, "@@ ")
            ItemsCount1 = 10000 \ items
            ItemsCount2 = 0
        End If
        If ItemsCount1 * ItemsCount2 > 10000 Then
            Return 10000
        Else
            Return ItemsCount1 * ItemsCount2
        End If
    End Function

    Private Sub CreateUpdate()
        If countItems(GetAllFiles(UpdateAPK, "*.apk")) = 0 Then Exit Sub
        CleanFolder(UpdateROM)

        ShowProgress(countItems(GetAllFiles(UpdateInput, "*.zip")))
        For Each file In GetAllFiles(UpdateInput, "*.zip")
            Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(file)
            If FileExists(InputRoms & "miuinl_" & filename & "-" & OldVersion.Text & ".zip") Then
                BackgroundWorker1.ReportProgress(ShowProgress(), "@@Update ZIP: " & "update_" & filename & "-" & NewVersion.Text & "_Signed.zip")
                xCopy(UpdateAPK, UpdateROM & "system\app")
                If OTAupdate.Checked Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & InputRoms & "miuinl_" & filename & "-" & OldVersion.Text & ".zip -o" & UpdateROM & "system build.prop -r -y"))
                    UpdaterScript = Replace(Template1.Text & UNIXtoDOS(System.IO.File.ReadAllText(UpdateROM & "system\build.prop")), "ro.build.version.incremental=" & OldVersion.Text, "ro.build.version.incremental=" & NewVersion.Text)
                    System.IO.File.WriteAllText(UpdateROM & "system\build.prop", DOStoUNIX(UpdaterScript))
                End If
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "copy " & file & " -> " & UpdateROM & "update_" & filename & "-" & NewVersion.Text & ".zip")
                My.Computer.FileSystem.CopyFile(file, UpdateROM & "update_" & filename & "-" & NewVersion.Text & ".zip", True)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & UpdateROM & "update_" & filename & "-" & NewVersion.Text & ".zip " & UpdateROM & "system -mx9"))
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -jar " & BaseFolder & "\_SIGN_ZIP\signapk.jar " & BaseFolder & "\_SIGN_ZIP\testkey.x509.pem " & BaseFolder & "\_SIGN_ZIP\testkey.pk8 " & UpdateROM & "update_" & filename & "-" & NewVersion.Text & ".zip " & UpdateZIP & "update_" & filename & "-" & NewVersion.Text & "_Signed.zip", BaseFolder))
                CleanFolder(UpdateROM)
            Else
                BackgroundWorker1.ReportProgress(ShowProgress(-2), InputRoms & "miuinl_" & filename & "-" & OldVersion.Text & ".zip NOT FOUND! Can't make update for this ROM!")
            End If
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Update ZIP: Ready")

    End Sub

    Private Sub OTAupdate_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OTAupdate.CheckedChanged
        If OTAupdate.Checked Then
            Label15.Text = "(Format: y.M.d)"
            NewVersion.Text = Mid(Format(Date.Today, "yy.M.d"), 2)
        Else
            Label15.Text = "(Format: yyyy-MM-dd)"
            NewVersion.Text = Format(Date.Today, "yyyy-MM-dd")
        End If
    End Sub

    Private Sub CreUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreUpdate.Click
        StartBackgroundWorker("CreateUpdate")
    End Sub

    Private Sub Logging_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Logging.TextChanged
        Logging.SelectionStart = InStrRev(Logging.Text, vbNewLine) + 1
        Logging.ScrollToCaret()
    End Sub

    Private Sub sortStrings_Click(sender As System.Object, e As System.EventArgs) Handles sortStrings.Click
        Me.sortStrings.Enabled = False
        StartBackgroundWorker("SortStrings")
    End Sub

    Private Sub srtStrings()
        Dim str As String
        Dim dictDoc11 As New Dictionary(Of String, String)
        Dim objReader12 As New ArrayList
        Dim keyName, TextResult As String
        Dim keyNameStart, keyNameEnd As Integer
        Dim multiLine As Boolean = False

        ShowProgress(countItems(GetAllSubFiles(LanguageTranslationFolder.Text, "strings.xml")))
        For Each file In GetAllSubFiles(LanguageTranslationFolder.Text, "strings.xml")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Sort: " & file)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Sort: " & file)
            dictDoc11.Clear()
            objReader12.Clear()
            Dim objReader1 As New System.IO.StreamReader(file.ToString, System.Text.Encoding.UTF8)
            multiLine = False
            Do While objReader1.Peek() <> -1
                str = objReader1.ReadLine()
                If InStr(str, "<string name=") <> 0 Then
                    keyNameStart = InStr(str, "name=") + 6
                    keyNameEnd = InStr(str, ">") - 1
                    keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                    If InStr(str, " />") <> 0 Then
                        objReader12.Add(Trim(str))
						If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (0): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    ElseIf InStr(str, "</string>") <> 0 Then
                        objReader12.Add(Trim(str))
						If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (1): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    Else
                        objReader12.Add(Trim(str))
                        multiLine = True
                    End If
                    'keys in current ROM
                Else
                    If multiLine Then
                        objReader12.Add(str)
                        If InStr(str, "</string>") <> 0 Then
                            multiLine = False
							If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                            BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (2): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                            objReader12.Clear()
                        End If
                    End If
                End If
            Loop
            objReader1.Close()

            Dim keys11 As List(Of String) = dictDoc11.Keys.ToList
            keys11.Sort()

            TextResult = ""
            For Each key11 In keys11
                For Each Line In dictDoc11.Item(key11).Split("þ")
                    TextResult = TextResult & Line & vbNewLine
                Next
            Next
            TextResult = Replace(TextResult, vbNewLine & vbNewLine, vbNewLine)
            TextResult = Replace(TextResult, "<string name=", "   <string name=")
            System.IO.File.WriteAllText(file.ToString, "<?xml version=""1.0"" encoding=""utf-8""?>" & vbNewLine & "<resources>" & vbNewLine & TextResult & "</resources>" & vbNewLine, System.Text.Encoding.UTF8)
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Sort: Ready")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        CheckedListBox2.Items.Clear()
        Dim di1 As New IO.DirectoryInfo(OutputRoms)
        Dim diarr1 As IO.FileInfo() = di1.GetFiles("*.zip")
        Dim dra1 As IO.FileInfo

        'list the names of all files in the specified directory
        For Each dra1 In diarr1
            CheckedListBox2.Items.Add(dra1)
        Next
    End Sub

    Private Sub CheckedListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckedListBox2.SelectedIndexChanged
        ListBox2.Items.Clear()
        If CheckedListBox2.CheckedItems.Count <> 0 Then
            ' If so, loop through all checked items and print results.
            Dim x As Integer
            For x = 0 To CheckedListBox2.CheckedItems.Count - 1
                ListBox2.Items.Add(CheckedListBox2.CheckedItems(x).ToString)
            Next x
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        StartBackgroundWorker("FTP")
    End Sub

    Private Sub uploadFTP(ByVal inFolder As String, ByVal sendFile As String)
        Label18.Text = inFolder & "/" & OldVersion.Text
        Try
            If Not System.Net.WebRequestMethods.Ftp.ListDirectoryDetails.Contains(inFolder & "/" & OldVersion.Text) Then
                Dim FTPReq As System.Net.FtpWebRequest = CType(System.Net.WebRequest.Create(inFolder & "/" & OldVersion.Text), System.Net.FtpWebRequest)
                FTPReq.Credentials = New System.Net.NetworkCredential(FTPu.Text, FTPp.Text)
                FTPReq.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory
                Dim FTPRes As System.Net.FtpWebResponse
                FTPRes = CType(FTPReq.GetResponse, System.Net.FtpWebResponse)
            End If
        Catch ex As Exception
            BackgroundWorker1.ReportProgress(0, ex.ToString)
        End Try
        Dim sendFilename As String = System.IO.Path.GetFileName(sendFile)
        Dim request As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(inFolder & "/" & OldVersion.Text & "/" & sendFilename), System.Net.FtpWebRequest)
        request.Credentials = New System.Net.NetworkCredential(FTPu.Text, FTPp.Text)
        request.Method = System.Net.WebRequestMethods.Ftp.UploadFile
        request.Timeout = 900000 '=15 minutes

        Dim file() As Byte = System.IO.File.ReadAllBytes(sendFile)

        Dim strz As System.IO.Stream = request.GetRequestStream()
        Dim intOffset As Integer = 0
        Dim intChunk, PBvalue As Integer
        intChunk = file.Length / 100
        'For I As Integer = 1 To 100
        While intOffset < file.Length
            If intOffset + intChunk > file.Length Then intChunk = file.Length - intOffset
            strz.Write(file, intOffset, intChunk)
            intOffset += intChunk
            PBvalue += 100
            If PBvalue > 10000 Then PBvalue = 10000
            BackgroundWorker1.ReportProgress(PBvalue, "@@FTP upload file: " & inFolder & "/" & OldVersion.Text & "/" & sendFilename & " @ " & PBvalue / 100 & "%")
            'Next
        End While
        strz.Close()
        strz.Dispose()

    End Sub

    Private Sub doSelectFTP(ByVal automatic As Boolean)
        If automatic Then
            For Each ROM In ListBox1.Items 'GetAllFiles(InputRoms, "*.zip")
                Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(ROM)
                uploadFTP(FTPlocation.Text, OutputRoms & filename & "_Signed.zip")
            Next
            autoUpload.ForeColor = Color.Lime
        Else
            For Each ROM In ListBox2.Items 'GetAllFiles(OutputRoms, "*.zip")
                uploadFTP(FTPlocation.Text, OutputRoms & ROM)
            Next

        End If

    End Sub

    Private Sub autoUpload_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles autoUpload.CheckedChanged
        If autoUpload.Checked Then MsgBox("Files will upload @ " & FTPlocation.Text & "/" & OldVersion.Text, MsgBoxStyle.OkOnly)
    End Sub

    Private Sub CheckedListBox3_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CheckedListBox3.SelectedIndexChanged
        ListBox3.Items.Clear()
        If CheckedListBox3.CheckedItems.Count <> 0 Then
            ' If so, loop through all checked items and print results.
            Dim x As Integer
            For x = 0 To CheckedListBox3.CheckedItems.Count - 1
                ListBox3.Items.Add(CheckedListBox3.CheckedItems(x).ToString)
            Next x
        End If

    End Sub

    Private Sub DoDeOdex_Click(sender As System.Object, e As System.EventArgs) Handles DoDeOdex.Click
        Me.DoDeOdex.Enabled = False
        StartBackgroundWorker("DeOdex")
    End Sub

    Private Sub deodex()
        Dim source, destination, APIlevel, BuildProp, AllJar As String
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "smali.jar: " & Smalijar)
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "baksmali.jar: " & BakSmalijar)
        APIlevel = 15
        ShowProgress(ListBox3.Items.Count)
        For Each ROM In ListBox3.Items 'GetAllFiles(InputRoms, "*.zip")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Processing ROM: " & ROM)
            Dim ROMname As String = System.IO.Path.GetFileName(ROM)
            Dim ROMnameNoExtension As String = System.IO.Path.GetFileNameWithoutExtension(ROM)
            If Not FolderExists(DeodexTemp & ROMnameNoExtension) Then System.IO.Directory.CreateDirectory(DeodexTemp & ROMnameNoExtension)
            CleanFolder(DeodexTemp & ROMnameNoExtension, True)
            source = DeodexTemp & ROMnameNoExtension & "\source"
            If Not FolderExists(source) Then System.IO.Directory.CreateDirectory(source)
            destination = DeodexTemp & ROMnameNoExtension & "\update\system"
            If Not FolderExists(destination) Then System.IO.Directory.CreateDirectory(destination)
            If Not FolderExists(destination & "\app") Then System.IO.Directory.CreateDirectory(destination & "\app")
            If Not FolderExists(destination & "\framework") Then System.IO.Directory.CreateDirectory(destination & "\framework")
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & OdexedRoms & ROMname & " -o" & source & " *.apk -r -y"))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & OdexedRoms & ROMname & " -o" & source & " *.jar -r -y"))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & OdexedRoms & ROMname & " -o" & source & " *.odex -r -y"))
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " e " & OdexedRoms & ROMname & " -o" & source & " build.prop -r -y"))

            BuildProp = UNIXtoDOS(System.IO.File.ReadAllText(source & "\build.prop"))
            For Each line In Split(BuildProp, vbNewLine)
                If InStr(line, "ro.build.version.sdk=") <> 0 Then APIlevel = Int(Mid(line, InStr(line, "=") + 1)).ToString
            Next
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ROM: " & ROM & ", API: " & APIlevel)

            If FileExists(source & "\build.prop") Then My.Computer.FileSystem.DeleteFile(source & "\build.prop")

            AllJar = ""
            Dim di1 As New IO.DirectoryInfo(source)
            Dim AllJars As IO.FileInfo() = di1.GetFiles("*.jar")
            Dim Jar As IO.FileInfo
            For Each Jar In AllJars
                AllJar = AllJar & ":" & Jar.ToString
            Next

            BackgroundWorker1.ReportProgress(ShowProgress(-2), "All jars: " & AllJar)

            Dim Allodexs As IO.FileInfo() = di1.GetFiles("*.odex")
            Dim odex As IO.FileInfo
            For Each odex In Allodexs
                Dim ODEXname As String = System.IO.Path.GetFileName(odex.ToString)
                Dim ODEXnameNoExtension As String = System.IO.Path.GetFileNameWithoutExtension(odex.ToString)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Processing ROM: " & ROM & ", API: " & APIlevel & ", deodex: " & odex.ToString)

                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoJava & " -Xmx512m -jar " & BakSmalijar & " -d " & source & " -c " & AllJar & " -x " & source & "\" & ODEXname & " -o " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoJava & " -Xmx512m -jar " & Smalijar & " " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel & " -o " & source & "\" & ODEXnameNoExtension & "\classes.dex")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -Xmx512m -jar " & BakSmalijar & " -d " & source & " -c " & AllJar & " -x " & source & "\" & ODEXname & " -o " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel, BaseFolder))
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -Xmx512m -jar " & Smalijar & " " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel & " -o " & source & "\" & ODEXnameNoExtension & "\classes.dex", BaseFolder))
                If Not FileExists(source & "\" & ODEXnameNoExtension & "\classes.dex") Then
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoJava & " -Xmx512m -jar " & BakSmalijar & " -d " & source & " -x " & source & "\" & ODEXname & " -o " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel)
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoJava & " -Xmx512m -jar " & Smalijar & " " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel & " -o " & source & "\" & ODEXnameNoExtension & "\classes.dex")
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -Xmx512m -jar " & BakSmalijar & " -d " & source & " -x " & source & "\" & ODEXname & " -o " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel, BaseFolder))
                    BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(DoJava, " -Xmx512m -jar " & Smalijar & " " & source & "\" & ODEXnameNoExtension & " -a " & APIlevel & " -o " & source & "\" & ODEXnameNoExtension & "\classes.dex", BaseFolder))
                End If
                If FileExists(source & "\" & ODEXnameNoExtension & "\classes.dex") Then
                    If FileExists(source & "\" & ODEXnameNoExtension & ".jar") Then
                        My.Computer.FileSystem.CopyFile(source & "\" & ODEXnameNoExtension & ".jar", destination & "\framework\" & ODEXnameNoExtension & ".jar", True)
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), BaseFolder & "\7za.exe" & " u -tzip " & destination & "\framework\" & ODEXnameNoExtension & ".jar " & source & "\" & ODEXnameNoExtension & "\classes.dex -mx9")
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " u -tzip " & destination & "\framework\" & ODEXnameNoExtension & ".jar " & source & "\" & ODEXnameNoExtension & "\classes.dex -mx9"))
                    ElseIf FileExists(source & "\" & ODEXnameNoExtension & ".apk") Then
                        My.Computer.FileSystem.CopyFile(source & "\" & ODEXnameNoExtension & ".apk", destination & "\app\" & ODEXnameNoExtension & ".apk", True)
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), BaseFolder & "\7za.exe" & " u -tzip " & destination & "\app\" & ODEXnameNoExtension & ".apk " & source & "\" & ODEXnameNoExtension & "\classes.dex -mx9")
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " u -tzip " & destination & "\app\" & ODEXnameNoExtension & ".apk " & source & "\" & ODEXnameNoExtension & "\classes.dex -mx9"))
                    End If
                    'If FileExists(source & "\" & ODEXnameNoExtension & ".odex") Then My.Computer.FileSystem.DeleteFile(source & "\" & ODEXnameNoExtension & ".odex")
                    'If FileExists(source & "\" & ODEXnameNoExtension & ".jar") Then My.Computer.FileSystem.DeleteFile(source & "\" & ODEXnameNoExtension & ".jar")
                    'If FileExists(source & "\" & ODEXnameNoExtension & ".apk") Then My.Computer.FileSystem.DeleteFile(source & "\" & ODEXnameNoExtension & ".apk")

                End If
            Next

            My.Computer.FileSystem.CopyFile(OdexedRoms & ROMname, DeOdexedRoms & ROMname, True)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), BaseFolder & "\7za.exe" & " a -tzip " & DeOdexedRoms & ROMname & " " & OdexedRoms & ROMnameNoExtension & "\update\* -mx9")
            BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & DeOdexedRoms & ROMname & " " & DeodexTemp & ROMnameNoExtension & "\update\* -mx9"))
        Next

        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@DeOdex ROM: Ready")
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##DeOdex ROM: Ready")
    End Sub

    Private Sub BakSmaliVersion_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles BakSmaliVersion.SelectedIndexChanged
        BakSmalijar = BaseFolder & "\bak" & BakSmaliVersion.Text
        Smalijar = BaseFolder & "\" & BakSmaliVersion.Text
        DoLog("BakSmali version: " & BakSmaliVersion.Text)

    End Sub

    Private Sub DebugSwitch_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles DebugSwitch.CheckedChanged
        If DebugSwitch.Checked Then
            DebugOn = True
        Else
            DebugOn = False
        End If
    End Sub

    Private Sub MakeTheme_Click(sender As System.Object, e As System.EventArgs) Handles MakeTheme.Click
        Me.MakeTheme.Enabled = False
        StartBackgroundWorker("Theme")

    End Sub

    Private Sub CreateTheme()
        Dim tablefile As String = System.IO.File.ReadAllText(Theme & "\build-mtz.txt")
        Dim dictDoc As New Dictionary(Of String, String)
        Dim miuistrings As String = ""

        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##build-mtz read")

        For Each line In Split(tablefile, vbNewLine)
            dictDoc.Add(Mid(line, 1, InStr(line, "=") - 1).ToString, Mid(line, InStr(line, "=") + 1).ToString)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), Mid(line, 1, InStr(line, "=") - 1) & "=" & Mid(line, InStr(line, "=") + 1))
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##dictionary filled")

        My.Computer.FileSystem.CopyFile(Theme & "miuinl.template.mtz", Theme & "miuinl.mtz", True)
        If FolderExists(Theme & "build") Then
            CleanFolder(Theme & "build", True)
        Else
            System.IO.Directory.CreateDirectory(Theme & "build")
        End If
        If FolderExists(Theme & "theme") Then
            CleanFolder(Theme & "theme", True)
        Else
            System.IO.Directory.CreateDirectory(Theme & "theme")
        End If
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "##build & theme created")

        ShowProgress(countItems(GetAllFolders(LanguageTranslationFolder.Text, "*")))
        For Each folder In GetAllFolders(LanguageTranslationFolder.Text, "*")
            FolderName = System.IO.Path.GetFileName(folder)
            If dictDoc.ContainsKey(FolderName) Then
                BackgroundWorker1.ReportProgress(ShowProgress(), FolderName)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@" & FolderName)
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "##" & FolderName)
                System.IO.Directory.CreateDirectory(Theme & "build\" & FolderName)
                If FolderExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\drawable-nl-hdpi-finger") Then
                    'System.IO.Directory.CreateDirectory(Theme & "build\" & FolderName & "\res\drawable-hdpi-finger")
                    My.Computer.FileSystem.CopyDirectory(LanguageTranslationFolder.Text & "\" & FolderName & "\res\drawable-nl-hdpi-finger", Theme & "build\" & FolderName & "\res\drawable-hdpi-finger", True)
                End If
                If FolderExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\drawable-nl-hdpi") Then
                    'System.IO.Directory.CreateDirectory(Theme & "build\" & FolderName & "\res\drawable-hdpi")
                    My.Computer.FileSystem.CopyDirectory(LanguageTranslationFolder.Text & "\" & FolderName & "\res\drawable-nl-hdpi", Theme & "build\" & FolderName & "\res\drawable-hdpi", True)
                End If

                BackgroundWorker1.ReportProgress(ShowProgress(-2), "##" & LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml")
                If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml") Then
                    miuistrings = System.IO.File.ReadAllText(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl\strings.xml")
                    miuistrings = Replace(miuistrings, "<resources>", "<MIUI_Theme_Values>")
                    miuistrings = Replace(miuistrings, "</resources>", "</MIUI_Theme_Values>")
                    System.IO.File.WriteAllText(Theme & "build\" & FolderName & "\theme_values.xml", miuistrings)
                End If
                BackgroundWorker1.ReportProgress(ShowProgress(-2), "##" & LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml")
                If FileExists(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml") Then
                    miuistrings = System.IO.File.ReadAllText(LanguageTranslationFolder.Text & "\" & FolderName & "\res\values-nl-rNL\strings.xml")
                    miuistrings = Replace(miuistrings, "<resources>", "<MIUI_Theme_Values>")
                    miuistrings = Replace(miuistrings, "</resources>", "</MIUI_Theme_Values>")
                    System.IO.File.WriteAllText(Theme & "build\" & FolderName & "\theme_values.xml", miuistrings)
                End If
                'zip app
                Dim zipName As String = dictDoc.Item(FolderName).ToString
                BackgroundWorker1.ReportProgress(ShowProgress(-2), BaseFolder & "\7za.exe" & " a -tzip " & Theme & "theme\" & zipName & " " & Theme & "build\" & FolderName & "\* -mx9")
                BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & Theme & "theme\" & zipName & " " & Theme & "build\" & FolderName & "\* -mx9"))
                '            System.IO.File.Move(Theme & "theme\" & zipName & ".zip", Theme & "theme\" & zipName)
            End If
        Next
        'zip theme
        BackgroundWorker1.ReportProgress(ShowProgress(-2), BaseFolder & "\7za.exe" & " a -tzip " & Theme & "miuinl.mtz " & Theme & "theme\* -mx9")
        BackgroundWorker1.ReportProgress(ShowProgress(-2), DoExecute(BaseFolder & "\7za.exe", " a -tzip " & Theme & "miuinl.mtz " & Theme & "theme\* -mx9"))
    End Sub

    Private Sub MergeStock_Click(sender As System.Object, e As System.EventArgs) Handles MergeStock.Click
        Me.MergeStock.Enabled = False
        StartBackgroundWorker("MergeStock")
    End Sub

    Private Sub MergeStockStrings()
        Dim str As String
        Dim dictDoc1 As New Dictionary(Of String, Integer)
        Dim dictDoc11 As New Dictionary(Of String, String)
        Dim objReader12 As New ArrayList
        Dim dictDoc2 As New Dictionary(Of String, Integer)
        Dim dictDoc21 As New Dictionary(Of String, String)
        Dim objReader22 As New ArrayList
        Dim keyName, TextResult As String
        Dim keyNameStart, keyNameEnd As Integer
        Dim multiLine As Boolean = False

        ShowProgress(countItems(GetAllSubFiles(LanguageTranslationFolder.Text, "strings.xml")))
        For Each file In GetAllSubFiles(LanguageTranslationFolder.Text, "strings.xml")
            BackgroundWorker1.ReportProgress(ShowProgress(), "@@Merge: " & file)
            BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Merge: " & file)
            dictDoc1.Clear()
            dictDoc11.Clear()
            objReader12.Clear()
            Dim objReader1 As New System.IO.StreamReader(file.ToString, System.Text.Encoding.UTF8)
            multiLine = False
            Do While objReader1.Peek() <> -1
                str = objReader1.ReadLine()
                If InStr(str, "<string name=") <> 0 Then
                    keyNameStart = InStr(str, "name=") + 6
                    keyNameEnd = InStr(str, ">") - 1
                    keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                    If InStr(str, " />") <> 0 Then
                        dictDoc1.Add(keyName, 0)
                        objReader12.Add(Trim(str))
                        If Not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (0): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    ElseIf InStr(str, "</string>") <> 0 Then
                        dictDoc1.Add(keyName, 1)
                        objReader12.Add(Trim(str))
                        If Not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (1): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    Else
                        objReader12.Add(Trim(str))
                        multiLine = True
                    End If
                    'keys in current ROM
                Else
                    If multiLine Then
                        objReader12.Add(str)
                        If InStr(str, "</string>") <> 0 Then
                            multiLine = False
                            dictDoc1.Add(keyName, 2)
                            If Not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                            BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (2): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                            objReader12.Clear()
                        End If
                    End If
                End If
            Loop
            objReader1.Close()


            dictDoc2.Clear()
            dictDoc21.Clear()
            objReader12.Clear()
            If FileExists(Replace(file.ToString, LanguageTranslationFolder.Text, StockTranslationFolder.Text)) Then
                Dim objReader2 As New System.IO.StreamReader(Replace(file.ToString, LanguageTranslationFolder.Text, StockTranslationFolder.Text), System.Text.Encoding.UTF8)
                multiLine = False
                Do While objReader2.Peek() <> -1
                    str = objReader2.ReadLine()
                    If InStr(str, "<string name=") <> 0 Then
                        keyNameStart = InStr(str, "name=") + 6
                        keyNameEnd = InStr(str, ">") - 1
                        keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                        If InStr(str, " />") <> 0 Then
                            dictDoc2.Add(keyName, 0)
                            objReader12.Add(Trim(str))
                            If Not dictDoc21.ContainsKey(keyName) Then dictDoc21.Add(keyName, Join(objReader12.ToArray, "þ"))
                            BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (0): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                            objReader12.Clear()
                        ElseIf InStr(str, "</string>") <> 0 Then
                            dictDoc2.Add(keyName, 1)
                            objReader12.Add(Trim(str))
                            If Not dictDoc21.ContainsKey(keyName) Then dictDoc21.Add(keyName, Join(objReader12.ToArray, "þ"))
                            BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (1): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                            objReader12.Clear()
                        Else
                            objReader12.Add(Trim(str))
                            multiLine = True
                        End If
                        'keys in current ROM
                    Else
                        If multiLine Then
                            objReader12.Add(str)
                            If InStr(str, "</string>") <> 0 Then
                                multiLine = False
                                dictDoc2.Add(keyName, 2)
                                If Not dictDoc21.ContainsKey(keyName) Then dictDoc21.Add(keyName, Join(objReader12.ToArray, "þ"))
                                BackgroundWorker1.ReportProgress(ShowProgress(-2), "KeyName (2): " & keyName & ":" & Join(objReader12.ToArray, "þ"))
                                objReader12.Clear()
                            End If
                        End If
                    End If
                Loop
                objReader2.Close()

                Dim keys11 As List(Of String) = dictDoc11.Keys.ToList
                keys11.Sort()

                TextResult = ""
                For Each key11 In keys11
                    If dictDoc2.ContainsKey(key11) Then
                        If dictDoc1.Item(key11) = dictDoc2.Item(key11) Then
                            For Each Line In dictDoc21.Item(key11).Split("þ")
                                TextResult = TextResult & Line & vbNewLine
                            Next
							if dictDoc11.Item(key11).ToString <> dictDoc21.Item(key11).ToString then
								BackgroundWorker1.ReportProgress(ShowProgress(-2), "##Key: " & key11 & " = " & dictDoc11.Item(key11).ToString & "=>" & dictDoc21.Item(key11).ToString)
							End If
                        Else
                            For Each Line In dictDoc11.Item(key11).Split("þ")
                                TextResult = TextResult & Line & vbNewLine
                            Next
                        End If
                    Else
                        For Each Line In dictDoc11.Item(key11).Split("þ")
                            TextResult = TextResult & Line & vbNewLine
                        Next
                    End If
                Next
                TextResult = Replace(TextResult, vbNewLine & vbNewLine, vbNewLine)
                TextResult = Replace(TextResult, "<string name=", "   <string name=")
                System.IO.File.WriteAllText(file.ToString, "<?xml version=""1.0"" encoding=""utf-8""?>" & vbNewLine & "<resources>" & vbNewLine & TextResult & "</resources>" & vbNewLine, System.Text.Encoding.UTF8)
            End If
        Next
        BackgroundWorker1.ReportProgress(ShowProgress(-2), "@@Merge: Ready")

    End Sub

    Private Sub TextBox2_Click(sender As Object, e As System.EventArgs) Handles StockTranslationFolder.Click
        FolderBrowserDialog1.Description = "Select the " & Language.Text & " Stock Strings Folder"

        ' Do not show the button for new folder
        FolderBrowserDialog1.ShowNewFolderButton = False

        Dim dlgResult As DialogResult = FolderBrowserDialog1.ShowDialog()

        If dlgResult = DialogResult.OK Then
            StockTranslationFolder.Text = FolderBrowserDialog1.SelectedPath()
            StockFolder = StockTranslationFolder.Text
        End If

    End Sub

End Class
