using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrashIt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<int> activeProcesses = new List<int>();
        List<ListViewItem> itemList = new List<ListViewItem>();
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            Process[] processes = Process.GetProcesses();
            ListViewItem lstViewItems = null;
            foreach (Process process in processes)
            {
                lstViewItems = new ListViewItem();
                lstViewItems.Text = process.ProcessName;
                lstViewItems.SubItems.Add(process.Id.ToString());
                lstViewItems.SubItems.Add((process.PagedMemorySize64 / 1024 / 1024).ToString() + " MB");
                listView1.Items.Add(lstViewItems);
                activeProcesses.Add(process.Id);
            }
        }
        private void RemoveOldProcesses()
        {
            List<int> removeMe = new List<int>();
            foreach (int i in activeProcesses)
            {
                // If fails to get PID, remove from active process list
                try
                {
                    Process.GetProcessById(i);
                }
                catch (Exception)
                {
                    removeMe.Add(i);
                }
            }
            // Separate loops to prevent error
            foreach (int deadPID in removeMe)
            {
                if (activeProcesses.Contains(deadPID))
                    activeProcesses.Remove(deadPID);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RemoveOldProcesses();

            Process[] processes = Process.GetProcesses();
            ListViewItem lstViewItems = null;
            foreach (Process process in processes)
            {
                bool exists = false;
                lstViewItems = new ListViewItem();
                lstViewItems.Text = process.ProcessName;
                lstViewItems.SubItems.Add(process.Id.ToString());
                lstViewItems.SubItems.Add((process.PagedMemorySize64 / 1024 / 1024).ToString() + " MB");
              
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.SubItems[1].Text == process.Id.ToString())
                    {

                        exists = true;
                    }
                    else if (!activeProcesses.Contains(int.Parse(lvi.SubItems[1].Text)))
                    {
                        listView1.Items.Remove(lvi);
                    }
                }
                if (exists)
                    continue;
                listView1.Items.Add(lstViewItems);
                activeProcesses.Add(process.Id);

            }
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a process!", "No process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    Process p = Process.GetProcessById(int.Parse(lvi.SubItems[1].Text));
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Couldn't kill the process\n" + ex.ToString(), "Couldn't kill process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a process!", "No process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    Process p = null;
                    try
                    {
                        p = Process.GetProcessById(int.Parse(lvi.SubItems[1].Text));
                    }
                    catch
                    {
                        continue;
                    }
                    string file = p.MainModule.FileName;
                    try
                    {
                        p.Kill();
                        Process.Start(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Couldn't restart the process\n" + ex.ToString(), "Couldn't restart process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a process!", "No process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    Process p = null;
                    try
                    {
                        p = Process.GetProcessById(int.Parse(lvi.SubItems[1].Text));
                    }
                    catch
                    {
                        continue;
                    }
                    string argument = "/select, \"" + p.MainModule.FileName + "\"";
                    try
                    {
                        Process.Start("explorer.exe", argument);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Couldn't restart the process\n" + ex.ToString(), "Couldn't restart process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


    }

}

