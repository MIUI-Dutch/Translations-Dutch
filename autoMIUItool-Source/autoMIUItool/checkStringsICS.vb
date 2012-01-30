Public Class checkStringsICS

    Inherits System.Windows.Forms.Form

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
    Dim CFS1, CFS2 As String
    Dim objReader11 As New ArrayList
    Dim objReader12 As New ArrayList
    Dim objReader21 As New ArrayList
    Dim objReader22 As New ArrayList


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

    Private Sub checkStringsICS_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim objReader1 As New System.IO.StreamReader(CFS1, System.Text.Encoding.UTF8)
        multiLine = False
        Do While objReader1.Peek() <> -1
            str = objReader1.ReadLine()
            If InStr(str, "<string name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                If InStr(str, " />") <> 0 Then
                    dictDoc1.Add(keyName, 0)
                    objReader12.Add(str)
					If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                    objReader12.Clear()
                ElseIf InStr(str, "</string>") <> 0 Then
                    dictDoc1.Add(keyName, 1)
                    objReader12.Add(str)
					If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                    objReader12.Clear()
                Else
                    dictDoc1.Add(keyName, 2)
                    objReader12.Add(str)
                    multiLine = True
                End If
                'keys in current ROM
            Else
                If multiLine Then
                    objReader12.Add(str)
                    If InStr(str, "</string>") <> 0 Then
                        multiLine = False
						If not dictDoc11.ContainsKey(keyName) Then dictDoc11.Add(keyName, Join(objReader12.ToArray, "þ"))
                        objReader12.Clear()
                    End If
                End If
            End If
        Loop
        objReader1.Close()

        Dim objReader2 As New System.IO.StreamReader(CFS2, System.Text.Encoding.UTF8)
        Do While objReader2.Peek() <> -1
            str = objReader2.ReadLine()
            If InStr(str, "<string name=") <> 0 Then
                keyNameStart = InStr(str, "name=") + 6
                keyNameEnd = InStr(str, ">") - 1
                keyName = Mid(str, keyNameStart, keyNameEnd - keyNameStart)
                If dictDoc2.ContainsKey(keyName) Then dictDoc2.Remove(keyName)
                If dictDoc21.ContainsKey(keyName) Then dictDoc21.Remove(keyName)
                If dictDoc1.ContainsKey(keyName) Then
                    If InStr(str, " />") <> 0 Then
                        dictDoc2.Add(keyName, 0)
                        objReader22.Add(str)
                        dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                        objReader22.Clear()
                    ElseIf InStr(str, "</string>") <> 0 Then
                        dictDoc2.Add(keyName, 1)
                        objReader22.Add(str)
                        dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                        objReader22.Clear()
                    Else
                        dictDoc2.Add(keyName, 2)
                        objReader22.Add(str)
                        multiLine = True
                    End If
                End If
            Else
                If multiLine Then
                    objReader22.Add(str)
                    If InStr(str, "</string>") <> 0 Then
                        multiLine = False
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
                For Each Line In dictDoc11.Item(key11).Split("þ")
                    TextBox1.Text = TextBox1.Text & Line & vbNewLine
                Next
            Else
                If dictDoc1.Item(key11) <> dictDoc2.Item(key11) Then
                    NoNewXMLkeyAddition = False
                    NewStringsCount += 1
                    For Each Line In dictDoc11.Item(key11).Split("þ")
                        TextBox1.Text = TextBox1.Text & Line & vbNewLine
                    Next
                    For Each Line In dictDoc21.Item(key11).Split("þ")
                        If dictDoc1.Item(key11) = 2 Then
                            Line = Replace(Line, "\n", vbNewLine)
                            Line = Replace(Line, "\N", vbNewLine)
                            Line = Replace(Line, """>", """>""")
                            Line = Replace(Line, "</string", """</string")
                        End If
                        TextBox2.Text = TextBox2.Text & Line & vbNewLine
                    Next
                End If
            End If
        Next

        Dim keys21 As List(Of String) = dictDoc21.Keys.ToList
        keys21.Sort()

        For Each key21 In keys21
            For Each Line In dictDoc21.Item(key21).Split("þ")
                If InStr(TextBox1.Text, key21 & """>") = 0 Then TextBox3.Text = TextBox3.Text & Line & vbNewLine
            Next
        Next

        If NoNewXMLkeyAddition Then
            System.IO.File.WriteAllText(CFS2, "<?xml version=""1.0"" encoding=""utf-8""?>" & vbNewLine & "<resources>" & vbNewLine & TextBox3.Text & "</resources>" & vbNewLine, System.Text.Encoding.UTF8)
            Me.Close()
        End If
        Me.Text = CFS2
        NewStrings.Text = NewStringsCount.ToString
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim Textbox1Arr As Array = TextBox1.Text.Split(Environment.NewLine)
        For Each add In Textbox1Arr
            If InStr(add, "<string name=") <> 0 Then
                keyNameStart = InStr(add, "name=") + 6
                keyNameEnd = InStr(add, ">") - 1
                keyName = Mid(add, keyNameStart, keyNameEnd - keyNameStart)
                If dictDoc2.ContainsKey(keyName) Then dictDoc2.Remove(keyName)
                If dictDoc21.ContainsKey(keyName) Then dictDoc21.Remove(keyName)
                If InStr(add, " />") <> 0 Or InStr(add, "</string>") <> 0 Then
                    dictDoc2.Add(keyName, 0)
                    objReader22.Add(add)
                    dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                    objReader22.Clear()
                Else
                    dictDoc2.Add(keyName, 2)
                    objReader22.Add(add)
                    multiLine = True
                End If
            Else
                If multiLine Then
                    objReader22.Add(add)
                    If InStr(add, "</string>") <> 0 Then
                        multiLine = False
                        dictDoc21.Add(keyName, Join(objReader22.ToArray, "þ"))
                        objReader22.Clear()
                    End If
                End If
            End If
        Next
        TextBox1.Text = ""

        Dim keys21 As List(Of String) = dictDoc21.Keys.ToList
        keys21.Sort()

        TextBox3.Text = ""
        For Each key21 In keys21
            Try
                For Each Line In dictDoc21.Item(key21).Split("þ")
                    TextBox3.Text = TextBox3.Text & Line & vbNewLine
                Next
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        System.IO.File.WriteAllText(CFS2, "<?xml version=""1.0"" encoding=""utf-8""?>" & vbNewLine & "<resources>" & vbNewLine & TextBox3.Text & "</resources>" & vbNewLine, System.Text.Encoding.UTF8)
        Me.Close()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.Close()
    End Sub

End Class