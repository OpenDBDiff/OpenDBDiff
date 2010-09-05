Namespace DBDiff
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class SQLRichTextBox
#Region "Windows Form Designer generated code "
        <System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
            MyBase.New()
            'This call is required by the Windows Form Designer.
            InitializeComponent()
            UserControl_Initialize()
        End Sub
        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
            If Disposing Then
                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(Disposing)
        End Sub
        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer
        Public ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents RichTextBox As System.Windows.Forms.RichTextBox
        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.RichTextBox = New System.Windows.Forms.RichTextBox
            Me.SuspendLayout()
            '
            'RichTextBox
            '
            Me.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.RichTextBox.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RichTextBox.Location = New System.Drawing.Point(0, 0)
            Me.RichTextBox.Name = "RichTextBox"
            Me.RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
            Me.RichTextBox.Size = New System.Drawing.Size(320, 240)
            Me.RichTextBox.TabIndex = 0
            Me.RichTextBox.Text = ""
            '
            'SQLRichTextBox
            '
            Me.Controls.Add(Me.RichTextBox)
            Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "SQLRichTextBox"
            Me.Size = New System.Drawing.Size(320, 240)
            Me.ResumeLayout(False)

        End Sub
#End Region
    End Class
End Namespace