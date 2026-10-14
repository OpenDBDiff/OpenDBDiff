using System.Windows;
using System.Windows.Controls;
using ScintillaNET;

namespace OpenDBDiff.Wpf.Controls
{
    /// <summary>
    /// Hosts the same SQL editor the WinForms front end uses (the Scintilla control from
    /// Scintilla5.NET) so that highlighting, indentation and the native lexer libraries behave
    /// identically in both front ends.
    /// </summary>
    /// <remarks>
    /// This is the one place where the WPF front end deliberately keeps a WinForms control, through
    /// WindowsFormsHost. A pure-WPF editor (AvalonEdit, for example) would be a larger change than
    /// this port, because the SQL lexer setup and the margin handling would have to be rewritten and
    /// re-verified; it is called out in the pull request as the next step rather than smuggled in.
    /// </remarks>
    public partial class SqlEditor : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SqlEditor),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextChanged));

        public static readonly DependencyProperty LexerNameProperty =
            DependencyProperty.Register(
                nameof(LexerName),
                typeof(string),
                typeof(SqlEditor),
                new PropertyMetadata("sql", OnLexerNameChanged));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(SqlEditor),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        private readonly Scintilla scintilla;
        private bool isSynchronising;

        public SqlEditor()
        {
            InitializeComponent();

            scintilla = new Scintilla
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                LexerName = "sql",
                ReadOnly = false
            };

            ApplyEditorStyle();

            scintilla.TextChanged += OnScintillaTextChanged;
            Host.Child = scintilla;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string LexerName
        {
            get { return (string)GetValue(LexerNameProperty); }
            set { SetValue(LexerNameProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editor = (SqlEditor)sender;

            if (editor.isSynchronising)
            {
                return;
            }

            editor.isSynchronising = true;

            try
            {
                editor.scintilla.Text = args.NewValue as string ?? string.Empty;
                editor.SetMarginWidth();
            }
            finally
            {
                editor.isSynchronising = false;
            }
        }

        private static void OnLexerNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editor = (SqlEditor)sender;
            editor.scintilla.LexerName = args.NewValue as string ?? "sql";
            editor.ApplyEditorStyle();
        }

        private static void OnIsReadOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((SqlEditor)sender).scintilla.ReadOnly = (bool)args.NewValue;
        }

        private void OnScintillaTextChanged(object? sender, System.EventArgs e)
        {
            if (isSynchronising)
            {
                return;
            }

            isSynchronising = true;

            try
            {
                Text = scintilla.Text;
                SetMarginWidth();
            }
            finally
            {
                isSynchronising = false;
            }
        }

        private void ApplyEditorStyle()
        {
            // ScintillaNET.Style is qualified because this file also sees System.Windows.Style.
            // Courier New at the size the WinForms panes used; StyleClearAll pushes the default face
            // onto every lexer style so keywords and strings do not fall back to a serif font.
            scintilla.StyleResetDefault();
            scintilla.Styles[ScintillaNET.Style.Default].Font = "Courier New";
            scintilla.Styles[ScintillaNET.Style.Default].Size = 11;
            scintilla.StyleClearAll();
            scintilla.Styles[ScintillaNET.Style.LineNumber].Font = "Courier New";
            scintilla.Styles[ScintillaNET.Style.LineNumber].Size = 9;
            scintilla.Margins[0].Width = 40;
        }

        /// <summary>
        /// Widens the line-number margin to fit the number of lines, which is what the WinForms
        /// extension method of the same name did.
        /// </summary>
        private void SetMarginWidth()
        {
            var digits = scintilla.Lines.Count.ToString(System.Globalization.CultureInfo.InvariantCulture).Length;
            scintilla.Margins[0].Width = 8 + (digits * 8);
        }
    }
}
