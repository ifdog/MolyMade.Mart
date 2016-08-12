using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MolyMade.FieldUI
{
    public partial class Mainform : Form
    {
        private Controller _controller;
        private SynchronizationContext _uiContext;
        private Dictionary<string, TabPage> tabPages = new Dictionary<string, TabPage>();
        private Dictionary<string,DataGridView> dataGridViews = new Dictionary<string, DataGridView>();
        private Dictionary<string,DataTable> dataTables = new Dictionary<string, DataTable>();
        public Mainform()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            _controller = new Controller(_uiContext,DataUpdate);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Eventsform eventsform = new Eventsform();
            eventsform.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _controller.Start();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            _controller.Stop();
        }

        private void DataUpdate(object o)
        {
            Dictionary<string, Dictionary<string, string>> _dict= o as Dictionary<string, Dictionary<string, string>>;
            foreach (string _name in _dict.Keys)
            {
                if (tabPages.ContainsKey(_name))
                {
                    dataTables[_name].Clear();
                    dataTables[_name].Columns.Clear();
                    dataTables[_name].Rows.Clear();
                    foreach (string k in _dict[_name].Keys)
                    {
                        dataTables[_name].Columns.Add(k, typeof(string));
                    }
                    DataRow dr = dataTables[_name].NewRow();
                    foreach (KeyValuePair<string, string> keyValuePair in _dict[_name])
                    {
                        dr[keyValuePair.Key] = keyValuePair.Value;
                    }
                    dataTables[_name].Rows.Add(dr);
                }
                else
                {
                    tabPages[_name] = new TabPage(_name);
                    tabControl1.TabPages.Add(tabPages[_name]);
                    dataGridViews[_name] = new DataGridView();
                    tabPages[_name].Controls.Add(dataGridViews[_name]);
                    dataGridViews[_name].Dock = DockStyle.Fill;
                    dataTables[_name] = new DataTable(_name);
                    dataGridViews[_name].DataSource = dataTables[_name];
                }

            }
        }
    }
}
