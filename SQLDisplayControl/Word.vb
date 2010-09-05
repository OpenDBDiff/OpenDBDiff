Option Strict Off
Option Explicit On
Namespace DBDiff
    Friend Class Word

        Public Enum WordClassType
            SQLWord = 1
            FunctionWord = 2
            StringWord = 3
            NumberWord = 4
            CommentWord = 5
            OperatorWord = 6
            SysTablesWord = 7
            TypeWord = 8
        End Enum

        Private oWordText As String
        Private oWordType As WordClassType
        Private oLen As Integer
        Public ReadOnly Property Size() As Integer
            Get
                Size = oLen
            End Get
        End Property


        Public Property Text() As String
            Get
                Text = oWordText
            End Get
            Set(ByVal Value As String)
                oWordText = Value
                oLen = Len(oWordText)
            End Set
        End Property


        Public Property WordType() As WordClassType
            Get
                WordType = oWordType
            End Get
            Set(ByVal Value As WordClassType)
                oWordType = Value
            End Set
        End Property

        Public ReadOnly Property IsOpenWordType() As Boolean
            Get
                IsOpenWordType = (oWordType = WordClassType.StringWord) Or (oWordType = WordClassType.CommentWord)
            End Get
        End Property
    End Class
End Namespace