Option Strict Off
Option Explicit On
Namespace DBDiff
    Public Class SQLRichTextBox
        Inherits System.Windows.Forms.UserControl
        Public Event TextChange()
        Private Const WM_SETREDRAW As Short = &HBS
        Private Const WM_USER As Short = &H400S
        Private Const EM_GETEVENTMASK As Integer = (WM_USER + 59)
        Private Const EM_SETEVENTMASK As Integer = (WM_USER + 69)

        'Private Declare Function LockWindowUpdate Lib "user32" (ByVal hwndLock As Integer) As Integer
        Private RTF As RTFGenerated
        Private IsEdit As Boolean
        Private oSQLType As SQLEnum.SQLTypeEnum
        Public Property SQLType() As SQLEnum.SQLTypeEnum
            Get
                SQLType = oSQLType
            End Get
            Set(ByVal Value As SQLEnum.SQLTypeEnum)
                oSQLType = Value
                RTF = New RTFGenerated(oSQLType)
            End Set
        End Property

        Public Overrides Property Text() As String
            Get
                Text = RichTextBox.Text
            End Get
            Set(ByVal Value As String)
                IsEdit = False
                RichTextBox.Rtf = RTF.BuildRTF(Value & vbCrLf & " ")
                IsEdit = True
                RaiseEvent TextChange()
            End Set
        End Property

        Private Sub RichTextBox_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles RichTextBox.TextChanged
            Dim iLastPosition As Integer
            Dim iLineStart As Integer
            Dim iLineEnd As Integer
            Dim iFound As Boolean
            Dim j As Integer
            If Len(RichTextBox.Text) >= 1 Then
                If IsEdit Then
                    iLastPosition = RichTextBox.SelectionStart
                    If iLastPosition > 0 Then
                        For j = iLastPosition To 1 Step -1
                            iFound = Mid(RichTextBox.Text, j, 2) = vbCrLf
                            If iFound Then
                                iLineStart = j + 2
                                Exit For
                            End If
                        Next
                        For j = iLastPosition To Len(RichTextBox.Text)
                            iFound = Mid(RichTextBox.Text, j, 2) = vbCrLf
                            If iFound Then
                                iLineEnd = j + 2
                                Exit For
                            End If
                        Next
                    End If
                    'LockWindowUpdate(RichTextBox.Handle.ToInt32)
                    RichTextBox.Rtf = RTF.BuildRTF((RichTextBox.Text))
                    RichTextBox.SelectionStart = iLastPosition
                    'LockWindowUpdate(0)
                End If
            End If
        End Sub

        Private Sub UserControl_Initialize()            
            SQLType = SQLEnum.SQLTypeEnum.SQLServer
            Me.Text = ""
        End Sub
    End Class
End Namespace