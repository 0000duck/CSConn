using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace socketClient_win {
    public partial class Form1 : Form {

        private Socket clientS;
        private byte[] result = new byte[1000];

        public Form1() {
            InitializeComponent();
            //this.ReceiveMsg();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        private void btn_conn_Click(object sender, EventArgs e) {
            IPEndPoint ipPoint = this.getServerAdd();
            if (clientS != null && clientS.Connected ==true) {
                String serverAddr = clientS.RemoteEndPoint.ToString();
                tb_history.AppendText("已连接 " + serverAddr + "\n");
                return;
            }
            this.conn(ipPoint);

        }

        /**
         * 连接服务器
         */
        public Boolean conn(IPEndPoint ipPoint) {
            clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                clientS.Connect(ipPoint);
                tb_history.AppendText("服务器连接成功1.1\n");
                Thread newThread = new Thread(ReceiveMsg);
                newThread.IsBackground = true;
                newThread.Start();
                return true;
            }
            catch (Exception ex) {
                tb_history.AppendText("连接失败\n失败原因：" + ex.Message.ToString());
                return false;
            }
        }

        /**
         * 获得服务器地址
         */
        public IPEndPoint getServerAdd() {
            String server_ip = tb_server_id.Text;
            String portStr = tb_port.Text;
            int port = int.Parse(portStr);

            IPAddress ip = IPAddress.Parse(server_ip);
            IPEndPoint ipPort = new IPEndPoint(ip, port);

            return ipPort;
        }

        private void btn_send_Click(object sender, EventArgs e) {
            String msg = this.getMsgContent();
            if (msg.Length == 0) {
                appendToHistory("请输入内容\n");
                return;
            }
            this.sendMsg(msg);
            tb_msg.Text = "";
        }



        /**
         * 发送消息
         */
        public void sendMsg(String msg) {
            try {
                byte[] msgByte = Encoding.UTF8.GetBytes(msg);
                clientS.Send(msgByte);
                tb_history.AppendText("本机:\n" + msg + "\n");
            }
            catch (Exception ex) {
                tb_history.AppendText("异常:" + ex.Message);
                if (clientS == null) {
                    return;
                }
                clientS.Shutdown(SocketShutdown.Both);
                clientS.Close();
            }

        }

        /**
         * 获得发送内容
         */
        public String getMsgContent() {
            String msg = tb_msg.Text;

            return msg;
        }

        /**
         * 接受服务端消息
         */
        public void ReceiveMsg() {
            while (true) {
                try {
                    int length = clientS.Receive(result);
                    string resultStr = Encoding.UTF8.GetString(result, 0, length);
                    appendToHistory("服务端：\n" + resultStr + "\n");
                }
                catch (Exception ex) {
                    appendToHistory("异常：\n" + ex.Message);
                    break;
                }
            }
        }

        private delegate void InvokeCallback(String msg);
        /**
         * 给历史记录增加文字
         */
        public void appendToHistory(String msg) {
            if (tb_history.InvokeRequired) {
                InvokeCallback callback = new InvokeCallback(appendToHistory);
                tb_history.Invoke(callback, msg);
            }
            else {
                if (tb_history == null) {
                    return;
                }
                Debug.WriteLine("");

                tb_history.AppendText(msg);
            }
        }
    }
}







