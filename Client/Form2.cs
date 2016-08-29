using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TcpServer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        
        public delegate void MyDelegate(object sender, EventArgs e);
        public event MyDelegate MyEvent;

        private void button1_Click(object sender, EventArgs e)
        {
            if (MyEvent != null)
            {
                string str = "addxxx" + textBox1.Text + "," + textBox2.Text + "," + textBox3.Text;
                sender = str;
                MyEvent(sender, new EventArgs());

                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
