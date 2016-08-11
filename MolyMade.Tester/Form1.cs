using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolyMade.FieldCommunication;

namespace MolyMade.Tester
{
    public partial class Form1 : Form
    {
        public RunningTag _Running = new RunningTag() {Value = true};
        private Comm c;
        public Form1()
        {
            InitializeComponent();
            c = new Comm(10);
            c.DataMount += C_DataMount;
        }

        private  void C_DataMount(object sender, DataMountEventArgs args)
        {
            var x = args;
        }

        private void button1_Click(object sender, EventArgs e)
        {     
            c.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c.Stop();
        }
    }
}
