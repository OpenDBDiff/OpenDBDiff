using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDBDiff.Settings;

namespace OpenDBDiff.Front
{
    public delegate void ListProjectHandler(Project itemSelected);

    public sealed partial class ListProjectsForm : Form
    {
        public event ListProjectHandler OnSelect;
        public event ListProjectHandler OnDelete;
        public event ListProjectHandler OnRename;

        private readonly List<Project> projects;

        public ListProjectsForm(List<Project> item)
        {
            InitializeComponent();
            projects = item;
            item.ForEach(pro =>
                             {
                                 ListViewItem listViewItem = new ListViewItem();
                                 listViewItem.ImageIndex = 0;
                                 listViewItem.Text = pro.Name;
                                 listView1.Items.Add(listViewItem);
                             });

            ContextMenu mnuContextMenu = new ContextMenu();
            ContextMenu = mnuContextMenu;
            MenuItem mnuItemDelete = new MenuItem();
            MenuItem mnuItemOpen = new MenuItem();
            MenuItem mnuItemRename = new MenuItem();
            mnuItemOpen.Text = "&Open";
            mnuItemRename.Text = "&Rename";
            mnuItemDelete.Text = "&Delete";
            mnuContextMenu.MenuItems.Add(mnuItemOpen);
            mnuContextMenu.MenuItems.Add(mnuItemRename);
            mnuContextMenu.MenuItems.Add(mnuItemDelete);
            mnuItemDelete.Click += new EventHandler(mnuItemDelete_Click);
            mnuItemOpen.Click += new EventHandler(mnuItemOpen_Click);
            mnuItemRename.Click += new EventHandler(mnuItemRename_Click);
            listView1.LabelEdit = true;
        }

        private void OpenProject()
        {
            try
            {
                if (listView1.SelectedItems.Count != 0)
                {
                    Project item = new Project();
                    item.ConnectionStringDestination = projects[listView1.SelectedItems[0].Index].ConnectionStringDestination;
                    item.ConnectionStringSource = projects[listView1.SelectedItems[0].Index].ConnectionStringSource;
                    item.Id = projects[listView1.SelectedItems[0].Index].Id;
                    item.Name = projects[listView1.SelectedItems[0].Index].Name;
                    item.Options = projects[listView1.SelectedItems[0].Index].Options;
                    item.Type = projects[listView1.SelectedItems[0].Index].Type;
                    if (OnSelect != null)
                        OnSelect(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteProject()
        {
            try
            {
                if (listView1.SelectedItems.Count != 0)
                {
                    DialogResult result = MessageBox.Show(this,
                                                          "Are you sure you want delete this Connection Project?",
                                                          "ATENTION", MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (OnDelete != null)
                            OnDelete(projects[listView1.SelectedItems[0].Index]);
                        projects.Remove(projects[listView1.SelectedItems[0].Index]);
                        listView1.Items.Remove(listView1.SelectedItems[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuItemRename_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                listView1.SelectedItems[0].BeginEdit();
            }
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                projects[listView1.SelectedItems[0].Index].Name = e.Label;
                if (OnRename != null)
                    OnRename(projects[listView1.SelectedItems[0].Index]);
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
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
