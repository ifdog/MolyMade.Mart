using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            var x = o as List<Dictionary<string, string>>;
            x?.ForEach(dict =>
            {
                string name = dict["_Name"];
                if (!dataTables.ContainsKey(name))
                {
                    tabPages[name] = new TabPage(name);
                    tabControl1.TabPages.Add(tabPages[name]);
                    dataGridViews[name] = new DataGridView();
                    tabPages[name].Controls.Add(dataGridViews[name]);
                    dataGridViews[name].Dock = DockStyle.Fill;
                    setDGV(dataGridViews[name]);
                    dataTables[name] = new DataTable(name);
                    dataGridViews[name].DataSource = dataTables[name];
                    dict.Keys.ToList().ForEach(k =>
                    {
                        dataTables[name].Columns.Add(k);
                    });
                }
                DataRow dr = dataTables[name].NewRow();
                dict.ToList().ForEach(kv =>
                {
                    dr[kv.Key] = kv.Value;
                });
                dataTables[name].Rows.Add(dr);
                if (dataTables[name].Rows.Count > 20)
                {
                    dataTables[name].Rows.RemoveAt(0);
                }
            });
            
        }
        public  void setDGV(DataGridView dgv)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, true, null);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = true;
            dgv.ReadOnly = true;
            dgv.AllowUserToOrderColumns = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            //dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.MultiSelect = false;
            dgv.AllowUserToOrderColumns = true;
        }
    }
}
