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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CollectorCallback cb = new CollectorCallback(cbfunciton);
            Controller c = new Controller(cb,_Running);
            c.Start();
            int x = 1;
        }

        public void cbfunciton(List<Dictionary<string, string>> p)
        {
            Dictionary<string,string> aa = new Dictionary<string, string>();

            var x = p;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fs = File.OpenRead("Mart.ini");
            StreamReader sr = new StreamReader(fs);
            string x = sr.ReadToEnd();
         //   byte[] bb = Encoding.GetEncoding("GB2312").GetBytes(x);
            string y = Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding("GB2312").GetBytes(x));
            int x1 = 1;
        }
    }
}
