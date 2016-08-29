using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace TcpServer
{
    public partial class Form1 : Form
    {
        IPAddress[] IP;//主机名
        IPAddress HostIP;
        IPEndPoint point;//IP地址和端口
        Socket socket;
        public string sendStr;
        Thread thread;
        Byte[] sendByte;
        public string infomation;
        delegate void SetTextCallback(string text);
        private void SetText0(string text)       //SetText方法，将添加到委托链
        {
            rtfShowMessage.AppendText(text);
        }
        private void SetText(string str)
        {
            this.Invoke(new SetTextCallback(SetText0), new object[] { str });
        }
        private void Proccess()
        {
            if (socket.Connected)
            {
                while (true)
                {
                    byte[] receiveByte = new byte[64];
                    try
                    {
                        socket.Receive(receiveByte, receiveByte.Length, 0);
                    }
                    catch (Exception)
                    {
                        SetText("连接已断开");
                        socket.Close();
                        thread.Abort();      
                        break;
                    }
                    string strInfo = Encoding.ASCII.GetString(receiveByte);
                    SetText(strInfo);
                }
            }
        }  
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            btnClose.Enabled = false;
            button1.Enabled = false;
            btnSendMessage.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //IP = Dns.GetHostAddresses(txtIP.Text);
            //IPAddress hostIP = IP[0];
            IPAddress hostIP = IPAddress.Parse(txtIP.Text);
            try
            {
                point = new IPEndPoint(hostIP,8001);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(point);
                thread = new Thread(new ThreadStart(Proccess));
                thread.IsBackground = true;
                thread.Start();
                btnStart.Enabled = false;
                btnClose.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                btnSendMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器没有开启!\n" + ex.Message);
                btnStart.Enabled = true;
            }
        }
        //发送
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            send();   
        }
        internal void send()
        {
            try
            {
                sendByte = new Byte[64];
                sendStr = rtfSendMessage.Text;
                rtfShowMessage.AppendText("\n本机:" + sendStr);
                sendByte = Encoding.ASCII.GetBytes(sendStr);
                socket.Send(sendByte, sendByte.Length, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       //关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                socket.Close();
                thread.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnStart.Enabled = true;
            button2.Enabled = false;
            btnClose.Enabled = false;
            button1.Enabled = false;
            btnSendMessage.Enabled = false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            sendStr = "select" + textBox1.Text;
            sendByte = Encoding.ASCII.GetBytes(sendStr);
            socket.Send(sendByte, sendByte.Length, 0);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

            f2.MyEvent += new Form2.MyDelegate(f2_MyEvent);
            DialogResult result = f2.ShowDialog();
        }

        void f2_MyEvent(object sender, EventArgs e)
        {
            string temp = sender.ToString();//获得f2传的参数
            sendByte = Encoding.ASCII.GetBytes(temp);
            socket.Send(sendByte, sendByte.Length, 0);
        }

    }
}
