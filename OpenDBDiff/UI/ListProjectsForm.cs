using OpenDBDiff.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public delegate void ListProjectHandler(Project itemSelected);

    public sealed partial class ListProjectsForm : Form
    {
        public event ListProjectHandler OnSelect;

        public event ListProjectHandler OnDelete;

        public event ListProjectHandler OnRename;

        private IList<Project> Projects { get; }

        private readonly string[] columnTitles;

        private int sortColumn = -1;

        private bool sortAscending = true;

        public ListProjectsForm(IEnumerable<Project> projects)
        {
            InitializeComponent();

            Projects = projects.ToList();
            columnTitles = ProjectsListView.Columns.Cast<ColumnHeader>().Select(c => c.Text).ToArray();

            PopulateList();

            if (Projects.Any())
            {
                ProjectsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                foreach (ColumnHeader column in ProjectsListView.Columns)
                    column.Width = Math.Max(column.Width, 100);
            }

            ProjectsListView.LabelEdit = true;
        }

        private static string GetColumnValue(Project project, int column)
        {
            switch (column)
            {
                case 1: return project.ConnectionStringSource;
                case 2: return project.ConnectionStringDestination;
                default: return project.ProjectName;
            }
        }

        private void PopulateList()
        {
            ProjectsListView.BeginUpdate();
            try
            {
                ProjectsListView.Items.Clear();

                if (!Projects.Any())
                {
                    ProjectsListView.Items.Add(new ListViewItem
                    {
                        Text = "There are no saved projects."
                    });
                    return;
                }

                var filter = FilterTextBox.Text.Trim();
                IEnumerable<Project> visible = Projects;
                if (filter.Length != 0)
                    visible = visible.Where(p => (p.ProjectName ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);

                if (sortColumn >= 0)
                {
                    visible = sortAscending
                        ? visible.OrderBy(p => GetColumnValue(p, sortColumn), StringComparer.CurrentCultureIgnoreCase)
                        : visible.OrderByDescending(p => GetColumnValue(p, sortColumn), StringComparer.CurrentCultureIgnoreCase);
                }

                foreach (var p in visible)
                    ProjectsListView.Items.Add(new ListViewItem(items: new string[] { p.ProjectName, p.ConnectionStringSource, p.ConnectionStringDestination }, imageIndex: 0) { Tag = p });
            }
            finally
            {
                ProjectsListView.EndUpdate();
            }
        }

        private Project SelectedProject
        {
            get
            {
                return ProjectsListView.SelectedItems.Count != 0
                    ? ProjectsListView.SelectedItems[0].Tag as Project
                    : null;
            }
        }

        private void OpenProject()
        {
            try
            {
                var selected = SelectedProject;
                if (selected != null)
                {
                    var item = new Project
                    {
                        Id = selected.Id,
                        ConnectionStringDestination = selected.ConnectionStringDestination,
                        ConnectionStringSource = selected.ConnectionStringSource,
                        ProjectName = selected.ProjectName,
                        Options = selected.Options,
                        Type = selected.Type,
                    };
                    OnSelect?.Invoke(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteProject()
        {
            try
            {
                var selected = SelectedProject;
                if (selected != null)
                {
                    if (MessageBox.Show(this,
                                        "Are you sure you want delete this project?",
                                        "Confirm project deletion", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        OnDelete?.Invoke(selected);
                        Projects.Remove(selected);
                        ProjectsListView.Items.Remove(ProjectsListView.SelectedItems[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuItemRename_Click(object sender, EventArgs e)
        {
            if (SelectedProject != null)
            {
                ProjectsListView.SelectedItems[0].BeginEdit();
            }
        }

        private void ProjectsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            var selected = SelectedProject;
            if (selected != null)
            {
                selected.ProjectName = e.Label.Trim();
                OnRename?.Invoke(selected);
            }
        }

        private void ProjectsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sortColumn == e.Column)
                sortAscending = !sortAscending;
            else
            {
                sortColumn = e.Column;
                sortAscending = true;
            }

            for (int i = 0; i < ProjectsListView.Columns.Count; i++)
                ProjectsListView.Columns[i].Text = i == sortColumn
                    ? columnTitles[i] + (sortAscending ? " ▲" : " ▼")
                    : columnTitles[i];

            var selected = SelectedProject;
            PopulateList();
            SelectProject(selected);
        }

        private void SelectProject(Project project)
        {
            if (project == null)
                return;

            var item = ProjectsListView.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == project);
            if (item != null)
            {
                // Re-selecting the same project would reload it in the main form, so suppress the event.
                ProjectsListView.SelectedIndexChanged -= ProjectsListView_SelectedIndexChanged;
                item.Selected = true;
                item.EnsureVisible();
                ProjectsListView.SelectedIndexChanged += ProjectsListView_SelectedIndexChanged;
            }
        }

        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && ProjectsListView.Items.Count != 0)
            {
                ProjectsListView.Focus();
                if (ProjectsListView.SelectedItems.Count == 0)
                    ProjectsListView.Items[0].Selected = true;
                ProjectsListView.FocusedItem = ProjectsListView.SelectedItems[0];
                e.Handled = true;
            }
        }

        private void ListProjectsForm_Shown(object sender, EventArgs e)
        {
            FilterTextBox.Focus();
        }

        private void mnuItemOpen_Click(object sender, EventArgs e)
        {
            OpenProject();
            Dispose();
        }

        private void mnuItemDelete_Click(object sender, EventArgs e)
        {
            DeleteProject();
        }

        private void ProjectsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void ProjectsListView_DoubleClick(object sender, EventArgs e)
        {
            Dispose();
        }

        private void ListProjectsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (FilterTextBox.Focused && FilterTextBox.TextLength != 0)
                    {
                        FilterTextBox.Clear();
                        e.Handled = e.SuppressKeyPress = true;
                        return;
                    }
                    Dispose();
                }
                if (e.KeyCode == Keys.Enter)
                {
                    // From the filter box, Enter opens the first matching project.
                    if (FilterTextBox.Focused && ProjectsListView.SelectedItems.Count == 0 && ProjectsListView.Items.Count != 0)
                        ProjectsListView.Items[0].Selected = true;
                    Dispose();
                }
                if (e.KeyCode == Keys.Delete && ProjectsListView.Focused)
                    DeleteProject();
            }
        }
    }
}
