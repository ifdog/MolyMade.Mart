using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolyMade.FieldCommunication;

namespace MolyMade.FieldUI
{
    public partial class Eventsform : Form
    {
        public Eventsform()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void MessageUpdate(object sender,MessageArriveArgs e)
        {
            var x = e.Messages;
        }
    }
}
