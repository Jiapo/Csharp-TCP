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
using System.Media;

namespace TcpServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int i = 0;  //连接客户端数
        int j;      //当前通信客户端的序号
        IPAddress HostIP;
        int Port;
        IPEndPoint point;
        Socket socket;
        Socket severSocket;
        private readonly List<Socket> sockets = new List<Socket>(); //给客户端发送消息的socket列表
        Thread threadwatch;

        //启动
        private void btnStart_Click(object sender, EventArgs e)
        {
            HostIP = IPAddress.Parse("127.0.0.1");
            Port = 8001;
            try
            {

                threadwatch = new Thread(ListenClientConnect);
                threadwatch.IsBackground = true;
                threadwatch.Start();
                SetText("服务器监听启动成功！\n");
                SetText(HostIP.ToString() + Port.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器启动失败！" + ex.Message);
            }
        }

        //监听   
        private void ListenClientConnect()
        {
            point = new IPEndPoint(HostIP, Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(point);
            socket.Listen(50);
            while (true)
            {
                severSocket= socket.Accept();
                SoundPlayer sp = new SoundPlayer(TcpServer.Properties.Resources.Msg);
                sp.Play();
                SetText("客户端连接成功！\n");
                sockets.Add(severSocket);
                AddClient();
                Thread thr = new Thread(ReceData);  //接收线程；
                thr.IsBackground = true;
                thr.Start();
                i++;
            }
        }

        //接收
        private void ReceData()
        {
            Socket tempRece = severSocket;
            try
            {
                while (true)
                {
                    byte[] receiveByte = new byte[64];
                    tempRece.Receive(receiveByte, receiveByte.Length, 0);

                    SoundPlayer sp = new SoundPlayer(TcpServer.Properties.Resources.QQ);
                    sp.Play();

                    string strInfo = Encoding.UTF8.GetString(receiveByte);
                    SetText(tempRece.RemoteEndPoint.ToString() + ":" + strInfo);
                }
            }
            catch (Exception)
            {
                DddClient(tempRece);
                SoundPlayer sp = new SoundPlayer(TcpServer.Properties.Resources.Water);
                sp.Play();
            }
        }

        //发送
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            int m = int.Parse(listView1.SelectedItems[0].Index.ToString());
            Byte[] sendByte = new Byte[64];
            string sendStr = "服务器端说：" + rtfSendMessage.Text + "\n";
            rtfShowMessage.AppendText("  " + sendStr);
            sendByte = Encoding.UTF8.GetBytes(sendStr.ToCharArray());
            sockets[m].Send(sendByte, sendByte.Length, 0);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnSendMessage_Click(sender, e);//回车键发送
            }
        }
        //关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                socket.Close();
                severSocket.Close();
            }
            catch
            {
            }
            finally
            {
                Close();
            }
        }
        //显示信息
        delegate void SetTextCallback(string text);
        private void SetText0(string text)
        {
            rtfShowMessage.AppendText(text + "");
        }
        private void SetText(string text)
        {
            this.Invoke(new SetTextCallback(SetText0), new object[] { text });
        }
        //添加IP列表
        delegate void ACDele();
        private void AddClient0()
        {
            int itemNum = listView1.Items.Count;
            string[] subItem = { i.ToString(), severSocket.RemoteEndPoint.ToString() };
            listView1.Items.Insert(itemNum, new ListViewItem(subItem));
            RefList();
        }
        private void AddClient()
        {
            this.Invoke(new ACDele(AddClient0));
        }
        //从IP列表删除
        delegate void DCDele(Socket tempSocket);
        private void DddClient0(Socket tempSocket)
        {
            int k = 0;
            for (; k < listView1.Items.Count; k++)
            {
                if (tempSocket.RemoteEndPoint.ToString() == listView1.Items[k].SubItems[1].Text)
                {
                    sockets.Remove(sockets[k]);
                    listView1.Items.Remove(listView1.Items[k]);
                    i--;
                    break;
                }
              
            }
            RefList();
        }
        private void DddClient(Socket tempSocket)
        {
            this.Invoke(new DCDele(DddClient0), new object[] { tempSocket });
        }
        //刷新列表序号
        private void RefList()
        {
            for (int k = 0; k < listView1.Items.Count; k++)
            {
                listView1.Items[k].SubItems[0].Text = (k + 1).ToString();
            }
        }
    }
}
