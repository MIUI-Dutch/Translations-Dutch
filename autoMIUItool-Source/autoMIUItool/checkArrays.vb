Public Class checkArrays

    Inherits System.Windows.Forms.Form

    Dim textCount1, textCount2 As Integer
    Dim CFS1, CFS2 As String

    Dim dictDoc1 As New Dictionary(Of String, Integer)
    Dim dictDoc2 As New Dictionary(Of String, Integer)
    Dim dictDoc11 As New Dictionary(Of String, String)
    Dim dictDoc21 As New Dictionary(Of String, String)
    Dim str, keyName, keyText, tmpRec As String
    Dim keyNameStart, keyNameEnd, keyTextStart, keyTextEnd As Integer
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


    Public Sub New(ByVal param1 As String, ByVal param2 As String)
        MyBase.New()
        InitializeComponent()
        File1 = param1
        File2 = param2
    End Sub

    Public WriteOnly Property File1() As String
        Set(ByVal Value As String)
            Me.CFS1 = Value
        End Set
    End Property

    Public WriteOnly Property File2() As String
        Set(ByVal Value As String)
            Me.CFS2 = Value
        End Set
    End Property

    Private Sub checkArrays_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
        TextBox1.Text = ""
        TextBox2.Text = ""
        For Each key11 In keys11
            For Each Line In dictDoc11.Item(key11).Split("þ")
                TextBox1.Text = TextBox1.Text & Line & vbNewLine
            Next
            If Not dictDoc2.ContainsKey(key11) Then
                NoNewXMLkeyAddition = False
                NewStringsCount += 1
                For Each Line In dictDoc11.Item(key11).Split("þ")
                    TextBox2.Text = TextBox2.Text & Line & vbNewLine
                Next
            Else
                If dictDoc1.Item(key11) <> dictDoc2.Item(key11) Then
                    NoNewXMLkeyAddition = False
                    NewStringsCount += 1
                    For Each Line In dictDoc11.Item(key11).Split("þ")
                        TextBox2.Text = TextBox2.Text & Line & vbNewLine
                    Next
                Else
                    For Each Line In dictDoc21.Item(key11).Split("þ")
                        TextBox2.Text = TextBox2.Text & Line & vbNewLine
                    Next
                End If
            End If
        Next
    End Sub

    'textCount1 = 0
    'textCount2 = 0

    'Dim objReader1 As New System.IO.StreamReader(CFS1, System.Text.Encoding.UTF8)
    'Do While objReader1.Peek() <> -1
    '    TextBox1.Text = TextBox1.Text & objReader1.ReadLine() & vbNewLine
    '    textCount1 = textCount1 + 1
    'Loop
    'objReader1.Close()

    'Dim objReader2 As New System.IO.StreamReader(CFS2, System.Text.Encoding.UTF8)
    'Do While objReader2.Peek() <> -1
    '    TextBox2.Text = TextBox2.Text & objReader2.ReadLine() & vbNewLine
    '    textCount2 = textCount2 + 1
    'Loop
    'objReader2.Close()

    'If textCount1 = textCount2 Then
    '    Me.Close()
    'End If
    'Me.Text = CFS2

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        System.IO.File.WriteAllText(CFS2, "<?xml version=""1.0"" encoding=""UTF-8""?>" & vbNewLine & "<resources>" & vbNewLine & TextBox2.Text & "</resources>" & vbNewLine, System.Text.Encoding.UTF8)
    End Sub
End Class