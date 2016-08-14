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
using MolyMade.FieldCommunication;

namespace MolyMade.FieldUI
{

    public partial class Eventsform : Form
    {
        public SynchronizationContext _uiContext;
        public Eventsform()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            setLB(listBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void MessageUpdate(object o)
        {
            var x = o as List<MessageItem>;
            var y = x?.Select(i => $"{i.Time.PadRight(10)}{i.Owner.PadRight(18)}{i.ThreadId.PadRight(5)}{i.Message}").ToList();
            y.ForEach(i =>
            {
                listBox1.Items.Insert(0,i);
            });
            if (listBox1.Items.Count > 100)
            {
                listBox1.Items.RemoveAt(100);
            }
        }

        public void setLB(ListBox lb)
        {
            Type dgvType = lb.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(lb, true, null);
         
        }
    }
}
