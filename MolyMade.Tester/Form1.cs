﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            aa["啊"]="是";
            var x = p;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _Running.Value = false;
        }
    }
}
