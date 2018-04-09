using OpenDBDiff.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDBDiff.Front
{
    public delegate void ListProjectHandler(Project itemSelected);

    public sealed partial class ListProjectsForm : Form
    {
        public event ListProjectHandler OnSelect;

        public event ListProjectHandler OnDelete;

        public event ListProjectHandler OnRename;

        private IList<Project> Projects { get; }

        public ListProjectsForm(IList<Project> projects)
        {
            InitializeComponent();

            Projects = projects;

            ProjectsListView.Items.Clear();

            if (Projects.Any())
            {
                foreach (var p in Projects)
                    ProjectsListView.Items.Add(new ListViewItem(items: new string[] { p.ProjectName, p.ConnectionStringSource, p.ConnectionStringDestination }, imageIndex: 0));

                ProjectsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                ProjectsListView.Items.Add(new ListViewItem
                {
                    Text = "There are no saved projects."
                });
            }

            ProjectsListView.LabelEdit = true;
        }

        private void OpenProject()
        {
            try
            {
                if (ProjectsListView.SelectedItems.Count != 0)
                {
                    var item = new Project
                    {
                        Id = Projects[ProjectsListView.SelectedItems[0].Index].Id,
                        ConnectionStringDestination = Projects[ProjectsListView.SelectedItems[0].Index].ConnectionStringDestination,
                        ConnectionStringSource = Projects[ProjectsListView.SelectedItems[0].Index].ConnectionStringSource,
                        ProjectName = Projects[ProjectsListView.SelectedItems[0].Index].ProjectName,
                        Options = Projects[ProjectsListView.SelectedItems[0].Index].Options,
                        Type = Projects[ProjectsListView.SelectedItems[0].Index].Type,
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
                if (ProjectsListView.SelectedItems.Count != 0)
                {
                    if (MessageBox.Show(this,
                                        "Are you sure you want delete this project?",
                                        "Confirm project deletion", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        OnDelete?.Invoke(Projects[ProjectsListView.SelectedItems[0].Index]);
                        Projects.Remove(Projects[ProjectsListView.SelectedItems[0].Index]);
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
            if (ProjectsListView.SelectedItems.Count != 0)
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

            if (ProjectsListView.SelectedItems.Count != 0)
            {
                Projects[ProjectsListView.SelectedItems[0].Index].ProjectName = e.Label.Trim();
                OnRename?.Invoke(Projects[ProjectsListView.SelectedItems[0].Index]);
            }
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
                    Dispose();
                if (e.KeyCode == Keys.Enter)
                    Dispose();
                if (e.KeyCode == Keys.Delete)
                    DeleteProject();
            }
        }
    }
}
