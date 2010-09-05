Option Strict Off
Option Explicit On
Namespace DBDiff
    Friend Class Words
        'local variable to hold collection
        Private mCol As Collection
        Public Function Add(ByRef Text As String, ByRef WordType As Word.WordClassType, Optional ByRef sKey As String = "") As Word
            'create a new object
            Dim objNewMember As Word
            objNewMember = New Word


            'set the properties passed into the method
            objNewMember.Text = Text
            objNewMember.WordType = WordType

            If Len(sKey) = 0 Then
                mCol.Add(objNewMember)
            Else
                mCol.Add(objNewMember, sKey)
            End If

            'return the object created
            Add = objNewMember
        End Function

        Default Public ReadOnly Property Item(ByVal vntIndexKey As Object) As Word
            Get
                Item = mCol.Item(vntIndexKey)
            End Get
        End Property



        Public ReadOnly Property Count() As Integer
            Get
                Count = mCol.Count()
            End Get
        End Property

        Public Sub Remove(ByRef vntIndexKey As Object)
            mCol.Remove(vntIndexKey)
        End Sub

        Public Sub New()
            MyBase.New()
            mCol = New Collection
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
End Namespace