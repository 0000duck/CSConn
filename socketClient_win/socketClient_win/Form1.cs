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
using System.Web.Script.Serialization;

namespace socketClient_win {
    public partial class Form1 : Form {

        private Socket clientS;
        private Socket listenS;
        private byte[] result = new byte[1000];
        private int maxCount = 10;
        private int listenPort = 51888;
        private int bytLength = 1000;

        public Form1() {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        private void btn_conn_Click(object sender, EventArgs e) {
            IPEndPoint ipPoint = this.getServerAdd();
            if (clientS != null && clientS.Connected == true) {
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
                
                //接受信息
                Thread newThread = new Thread(ReceiveServerMsg);
                newThread.IsBackground = true;
                newThread.Start();

                ////接受连接 --- startList(Object obj) 是类型不安全的方式，有待改进
                //Thread connThread = new Thread(startListen);
                //connThread.IsBackground = true;
                //connThread.Start();

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

        /**
         * 获得发送对象地址 
         */
        public IPEndPoint getTargetAdd(String ipAndPort) {
            String[] ipInfo = ipAndPort.Split(':');
            String sendIp = ipInfo[0];
            string portStr = ipInfo[1];
            int port = int.Parse(portStr);

            IPAddress ip = IPAddress.Parse(sendIp);
            IPEndPoint ipPort = new IPEndPoint(ip, port);

            return ipPort;
        }

        private void btn_send_Click(object sender, EventArgs e) {
            String msg = this.getMsgContent();
            if (msg.Length == 0) {
                appendToHistory("请输入内容\n");
                return;
            }
            this.sendMsg2OnlineCli(msg);
            this.sendMsg2Server(msg);
            tb_msg.Text = "";
        }

        /**
         * 发送信息给在线客户端
         */
        public void sendMsg2OnlineCli(String msg) {
            List<String> target = this.getSendTarget();

            foreach (String ipAndPort in target) {
                Socket so = this.connectTarget(ipAndPort);

                Debug.WriteLine("sends");
                sendMsg(msg, so);
            }

        }
        
        /**
        * 获得发送目标
        */
        public List<String> getSendTarget() {
            List<string> sendTarget = new List<string>();
            for (int i = 0; i < checked_lb_online.Items.Count; i++) {
                if (checked_lb_online.GetItemChecked(i) == false) {
                    continue;
                }
                String clientInfo = checked_lb_online.GetItemText(checked_lb_online.Items[i]);
                String[] info = clientInfo.Split('.');

                //此处 +2 是魔术数字，要根据现实界面而更改
                String ipAndPort = clientInfo.Substring(info[0].Length + 2);
                sendTarget.Add(ipAndPort);
            }
            return sendTarget;
        }


        /**
         * 和发送目标建立连接, 并返回连接的Socket
         */
        public Socket connectTarget(string ipAndPort) {
            String[] info = ipAndPort.Split(':');
            String ipStr = info[0];
            String portStr = info[1];
            int port = int.Parse(portStr);

            Debug.WriteLine("connect");
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipPort = new IPEndPoint(ip, port);

            Socket soOther = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soOther.Connect(ipPort);

            return soOther;
        }

        /**
         * 发送消息 
         */
        public void sendMsg(String msg, Socket so) {
            try {
                so.Send(Encoding.UTF8.GetBytes(msg));
                appendToHistory("发给:\n" + so.RemoteEndPoint.ToString() + "\n");
            }
            catch (Exception ex) {
                appendToHistory("send异常" + ex.Message);
                if (so == null) {
                    return;
                }
                so.Shutdown(SocketShutdown.Both);
                so.Close();
            }
        }

        /**
         * 发送消息
         */
        public void sendMsg2Server(String msg) {
            try {
                clientS.Send(Encoding.UTF8.GetBytes(msg));
                tb_history.AppendText("本机:\n" + msg + "\n");
            }
            catch (Exception ex) {
                tb_history.AppendText("sendServer异常:" + ex.Message);
                if (clientS == null) {
                    return;
                }
                clientS.Shutdown(SocketShutdown.Both);
                clientS.Close();
            }
        }

        /**
         * 发送消息
         */
        public void sendMsgData(MsgData md) {

        }

        /**
         * 获得发送内容
         */
        public String getMsgContent() {
            String msg = tb_msg.Text;

            return msg;
        }

        /**
         * 启动监听
         * obj 应该是 IPEndPoint 对象
         */
        public void startListen() {
            //IPAddress[] ipArr = Dns.GetHostAddresses(Dns.GetHostName());
            String ipStr = "127.0.0.1";
            IPAddress localIp = IPAddress.Parse(ipStr);
            Port portObj = new Port();
            listenPort = portObj.GetFirstAvailablePort();
            IPEndPoint ipPoint = new IPEndPoint(localIp, listenPort);

            listenS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenS.Bind(ipPoint);
            listenS.Listen(maxCount);

            Thread newThread = new Thread(listenConn);
            newThread.IsBackground = true;
            newThread.Start();

        }

        /**
         * 监听连接
         */
        public void listenConn() {
            while (true) {
                Socket aSocket = listenS.Accept();

                appendToHistory("连接来自[" + aSocket.RemoteEndPoint.ToString() + "]\n");
                this.sendMsg("这里是" + aSocket.LocalEndPoint.ToString() + "\n", aSocket);

                Thread newThread = new Thread(ReceiveMsg);
                newThread.IsBackground = true;
                newThread.Start(aSocket);
            }
        }

        /**
         * 接受信息
         */
        public void ReceiveMsg(Object obj) {
            //Socket aSocket = (Socket)obj;
            //while (true) {
            //    try {
            //        Byte[] res = new Byte[bytLength];
            //        int length = aSocket.Receive(res);
            //        String resString = Encoding.UTF8.GetString(res, 0, length);
            //        appendToHistory("来自" + aSocket.RemoteEndPoint.ToString() + "\n" + resString);
            //    }
            //    catch (Exception ex) {
            //        appendToHistory("receive异常：" + ex.Message + "\n");
            //        appendToHistory("断开与" + aSocket.RemoteEndPoint.ToString() + "连接\n");
            //        aSocket.Shutdown(SocketShutdown.Both);
            //        aSocket.Close();
            //        break;
            //    }
            //}
        }

        /**
         * 接受服务端消息
         */
        public void ReceiveServerMsg() {
            while (true) {
                try {
                    int length = clientS.Receive(result);
                    string resultStr = Encoding.UTF8.GetString(result, 0, length);
                    Debug.WriteLine("RECEIVE" + resultStr);

                    MsgData md = this.DeserializeMsg(resultStr);

                    if (md.cliList.Length > 0) {
                        Debug.WriteLine("cli list ");
                        showOnline(md.cliList);
                    }
                    if (md.msg.Length > 0) {
                        Debug.WriteLine("msg " );
                        appendToHistory("服务端：\n" + md.msg + "\n");
                    }
                }
                catch (Exception ex) {
                    appendToHistory("服务端 - Receive异常：\n" + ex.Message + "\n");
                    //appendToHistory("断开与" + clientS.RemoteEndPoint.ToString() + "连接\n");
                    clientS.Shutdown(SocketShutdown.Both);
                    clientS.Close();
                    break;
                }
            }
        }

        /**
         * 反序列数据
         */
        public MsgData DeserializeMsg(String mdString) {
            JavaScriptSerializer json = new JavaScriptSerializer();
            MsgData md = json.Deserialize<MsgData>(mdString);

            Debug.WriteLine("deserial Msg");
            return md;
        }

        /**
         * 显示在线客户端
         */
        public void showOnline(String cliListStr) {
            clearOnlieList_lb();
            String[] cliArr = cliListStr.Split('&');
            for (int i = 0; i < cliArr.Length; i++) {
                String cliInfo = i + ". " + cliArr[i];
                addToOnlieList_lb(cliInfo);
            }
        }

        private delegate void addListInvokeCallback(String msg);
        /**
         * 增加客户列表 ListBox
         */
        public void addToOnlieList_lb(String msg) {
            if (checked_lb_online.InvokeRequired) {
                addListInvokeCallback callback = new addListInvokeCallback(addToOnlieList_lb);
                checked_lb_online.Invoke(callback, msg);
            }
            else {
                checked_lb_online.Items.Add(msg);
            }
        }

        private delegate void listInvokeCallback();
        /**
         * 清空客户端列表 ListBox
         */
        public void clearOnlieList_lb() {
            if (checked_lb_online.InvokeRequired) {
                listInvokeCallback callback = new listInvokeCallback(clearOnlieList_lb);
                checked_lb_online.Invoke(callback);
            }
            else {
                checked_lb_online.Items.Clear();
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

                tb_history.AppendText(msg);
            }
        }
    }
}







