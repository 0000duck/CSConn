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

namespace socketServer_win {
    public partial class Form1 : Form {

        private string ipStr = "127.0.0.1";
        private Socket serverS;
        private int maxCliCount = 10;
        private int byteLength = 1000;
        private List<Socket> clientList = new List<Socket>();
        private List<Socket> cliList_live;

        public Form1() {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) {
            this.startServer();
        }

        public void startServer() {
            int port = this.getPort();
            this.startServer(port);
        }

        /**
         * 启动服务
         */
        public void startServer(int port) {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipPoint = new IPEndPoint(ip, port);
            if (serverS != null) {
                String serverAddr = serverS.LocalEndPoint.ToString();
                tb_history.AppendText("已启动监听 " + serverAddr + "\n");
                return;
            }

            serverS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverS.Bind(ipPoint);
            serverS.Listen(maxCliCount);
            tb_history.AppendText("启动监听" + serverS.LocalEndPoint.ToString() + "\n");

            Thread newThread = new Thread(listenClientConn);
            newThread.IsBackground = true;
            newThread.Start();
        }

        /**
         * 监听客户端
         */
        public void listenClientConn() {
            while (true) {
                Socket aSocket = serverS.Accept();

                clientList.Add(aSocket);
                showCliList();
                appendToHistory("连接来自" + aSocket.RemoteEndPoint.ToString() + "\n");

                Byte[] msgByte = Encoding.UTF8.GetBytes("这里是服务端");
                aSocket.Send(msgByte);

                Thread newThread = new Thread(RecieveMsg);
                newThread.IsBackground = true;
                newThread.Start(aSocket);
            }
        }

        /**
         * 接受信息
         */
        public void RecieveMsg(Object obj) {
            Socket aSocket = (Socket)obj;
            while (true) {
                try {
                    Byte[] res = new Byte[byteLength];
                    int length = aSocket.Receive(res);
                    String resString = Encoding.UTF8.GetString(res, 0, length);
                    appendToHistory("来自" + aSocket.RemoteEndPoint.ToString() + "\n" + resString + "\n");
                }
                catch (Exception ex) {
                    appendToHistory("异常：" + ex.Message + "\n");
                    aSocket.Shutdown(SocketShutdown.Both);
                    aSocket.Close();
                    this.showCliList();
                    break;
                }
            }
        }

        /**
         * 获得端口号
         */
        public int getPort() {
            String portStr = tb_port.Text;
            int port = int.Parse(portStr);

            return port;
        }

        private void tb_history_TextChanged(object sender, EventArgs e) {

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
                tb_history.AppendText(msg);
            }
        }

        /**
         * 增加客户端列表 ListBox
         */
        public void addToClientList_lb(String msg) {
            if (checked_lb_client.InvokeRequired) {
                InvokeCallback callback = new InvokeCallback(addToClientList_lb);
                checked_lb_client.Invoke(callback, msg);
            }
            else {
                checked_lb_client.Items.Add(msg);
            }
        }

        /**
         * 清空客户端列表 ListBox
         */
        private delegate void listInvokeCallback();
        public void clearClientList_lb() {
            if (checked_lb_client.InvokeRequired) {
                listInvokeCallback callback = new listInvokeCallback(clearClientList_lb);
                checked_lb_client.Invoke(callback);
            }
            else {
                checked_lb_client.Items.Clear();
            }
        }




        /**
         * 显示连接状态的客户端
         */
        public void showCliList() {
            clearClientList_lb();
            cliList_live = this.getConnctedCli();

            for (int i = 0; i < cliList_live.Count; i++) {
                String cliAddress = cliList_live[i].RemoteEndPoint.ToString();
                String cliInfo = i + ". " + cliAddress;
                addToClientList_lb(cliInfo);
            }
        }

        /**
         * 获取连接状态中的客户端
         */
        public List<Socket> getConnctedCli() {
            List<Socket> cliList = new List<Socket>();
            foreach (Socket so in clientList) {
                if (so.Connected && so.Poll(-1, SelectMode.SelectWrite)) {
                    cliList.Add(so);
                }
            }

            return cliList;
        }

        private void button1_Click(object sender, EventArgs e) {
            String msg = getMsgContent();
            List<Socket> sendList = this.getSendTarget();
            if (sendList.Count == 0) {
                appendToHistory("请选择发送对象\n");
                return;
            }
            this.sendMsg(msg, sendList);

            //显示发送信息
            tb_history.AppendText("服务端 - 发往");
            foreach (Socket so in sendList) {
                tb_history.AppendText(so.RemoteEndPoint.ToString() + ", ");
            }
            tb_history.AppendText("\n");
            tb_history.AppendText(msg + "\n");
            tb_msg.Clear();
        }

        /**
         * 发送消息
         */
        public void sendMsg(String msgStr, List<Socket> sendList) {
            foreach (Socket so in sendList) { 
                Byte[] msgByte = Encoding.UTF8.GetBytes(msgStr);
                so.Send(msgByte);
            }
        }

        /**
         * 获得发送对象
         */
        public List<Socket> getSendTarget() {
            List<Socket> sendList = new List<Socket>();
            for (int i = 0; i < checked_lb_client.Items.Count; i++) {
                if (checked_lb_client.GetItemChecked(i) == false) {
                    continue;
                }
                String clientInfo = checked_lb_client.GetItemText(checked_lb_client.Items[i]);
                String[] info = clientInfo.Split('.');
                int index = int.Parse(info[0]);
                sendList.Add(cliList_live[index]);
            }

            return sendList;
        }

        /**
         * 获取发送内容
         */
        public String getMsgContent() {
            String msg = tb_msg.Text;

            return msg;
        }
    }
}
