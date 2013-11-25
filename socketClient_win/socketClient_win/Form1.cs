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
using System.IO;

namespace socketClient_win {
    public partial class Form1 : Form {

        private Socket clientS;
        private Socket listenS;
        private byte[] result = new byte[1000];
        private int maxCount = 10;
        private int listenPort;
        private int bytLength = 1000;

        private StreamReader streamR;
        private StreamWriter streamW;

        public Form1() {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        /************************************************************** Socket 连接服务器 *********/

        /**
         * clientS 是否已连接
         */
        public Boolean isclientSConnected() {
            if (clientS != null && clientS.Connected == true) {
                return true;
            }
            return false;
        }

        /**
         * 连接服务器
         */
        public void conn(IPEndPoint ipPoint) {
            clientS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientS.Connect(ipPoint);

            NetworkStream ns = new NetworkStream(clientS);
            streamR = new StreamReader(ns);
            streamW = new StreamWriter(ns);

        }

        /**
         * 关闭和服务器的Socket
         */
        public void closeClientS() {
            clientS.Shutdown(SocketShutdown.Both);
            clientS.Close();
        }

        /**
        * 关闭指定的Socket
        */
        public void closeTheClose(Socket so) {
            so.Shutdown(SocketShutdown.Both);
            so.Close();
        }

        /*************************************************************** END Socket 连接服务器 ********/





        /**************************************************** Socket 接受消息 *************/////////////

        /**
        * 接受服务端信息
        */
        public MsgData receiveServerMsgData() {
            string resultStr = streamR.ReadLine();
            MsgData md = this.DeserializeMsg(resultStr);

            return md;
        }

        /**
        * 反序列数据
        */
        public MsgData DeserializeMsg(String mdString) {
            JavaScriptSerializer json = new JavaScriptSerializer();
            MsgData md = json.Deserialize<MsgData>(mdString);

            return md;
        }

        /**
        * 接受信息  
        */
        public void ReceiveMsg(Object obj) {
            Socket aSocket = (Socket)obj;
            while (true) {
                try {
                    Byte[] res = new Byte[bytLength];
                    int length = aSocket.Receive(res);
                    String resString = Encoding.UTF8.GetString(res, 0, length);

                    appendToHistory("来自" + aSocket.RemoteEndPoint.ToString() + "\n" + resString + "\n");
                }
                catch (Exception ex) {
                    appendToHistory("receive异常：" + ex.Message + "\n");
                    appendToHistory("断开与" + aSocket.RemoteEndPoint.ToString() + "连接\n");
                    aSocket.Shutdown(SocketShutdown.Both);
                    aSocket.Close();
                    break;
                }
            }
        }

        /**************************************************** Socket 接受消息 *************/////////////





        /*************************************************************** Socket 发送信息 ***************/

        public void clientSsend(string msg) {
            clientS.Send(Encoding.UTF8.GetBytes(msg));
        }

        public Socket getClientS() {
            return this.clientS;
        }

        /**
         * 发送信息给在线客户端  
         * 返回 List<string> failedList
         */
        public List<string> sendMsg2OnlineCli(String msg) {
            List<string> failedList = new List<string>();

            List<IPEndPoint> target = this.getSendTarget();

            foreach (IPEndPoint ipPort in target) {
                Socket so = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try {
                    so.Connect(ipPort);
                    so.Send(Encoding.UTF8.GetBytes(msg));
                }
                catch (Exception ex) {
                    String ipAndPort = "";  //ip address ----- 
                    string error = ipAndPort + "发送失败\n" + ex.Message;
                    failedList.Add(error);

                    if (so == null) {
                        continue;
                    }
                    this.closeTheClose(so);
                }
            }

            return failedList;
        }

        /**
        * 获得发送目标
        */
        public List<IPEndPoint> getSendTarget() {
            List<IPEndPoint> sendTarget = new List<IPEndPoint>();
            for (int i = 0; i < checked_lb_online.Items.Count; i++) {
                if (checked_lb_online.GetItemChecked(i) == false) {
                    continue;
                }
                String clientInfo = checked_lb_online.GetItemText(checked_lb_online.Items[i]);
                String[] info = clientInfo.Split('.');

                //此处 +2 是魔术数字，要根据现实界面而更改
                String ipAndPort = clientInfo.Substring(info[0].Length + 2);
                IPEndPoint ipPoint = this.translateIpEndPoint(ipAndPort);

                sendTarget.Add(ipPoint);
            }
            return sendTarget;
        }

        /**
         * String 转换为 IPEndPoint
         */
        public IPEndPoint translateIpEndPoint(String ipAndPort) {
            String[] info = ipAndPort.Split(':');
            String ipStr = info[0];
            String portStr = info[1];
            int port = int.Parse(portStr);

            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipPort = new IPEndPoint(ip, port);

            return ipPort;
        }



        /*************************************************************** END Socket 发送信息 ***********/



        /*************************************************************** 监听等待连接 ******************/
        /*************************************************************** 和连接 Server 同一接口 ********/

        /**
         * 启动监听
         * obj 应该是 IPEndPoint 对象
         */
        public void startListen_thread() {
            IPAddress[] ipArr = Dns.GetHostAddresses(Dns.GetHostName());

            //String ipStr = "127.0.0.1";
            //IPAddress localIp = IPAddress.Parse(ipStr);
            IPAddress localIp = ipArr[1];
            Port portObj = new Port();

            int localPort = getLocalPort(clientS);
            IPEndPoint ipPoint = new IPEndPoint(localIp, localPort);

            listenS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenS.Bind(ipPoint);
            listenS.Listen(maxCount);

            Thread newThread = new Thread(listenConn);
            newThread.IsBackground = true;
            newThread.Start();
        }

        /**
         * 获得和服务器连接的端口
         */
        public int getLocalPort(Socket so) {
            String ipAndPort = so.LocalEndPoint.ToString();
            String[] info = ipAndPort.Split(':');
            String portStr = info[1];
            int port = int.Parse(portStr);

            return port;
        }



        /**
         * 监听连接
         */
        public void listenConn() {
            while (true) {
                Socket aSocket = listenS.Accept();
                appendToHistory("连接来自[" + aSocket.RemoteEndPoint.ToString() + "]\n");

                //sendMsg(String s, Socket so);方法已删除，自己在生成
                //this.sendMsg("这里是" + aSocket.LocalEndPoint.ToString() + "\n", aSocket);

                Thread newThread = new Thread(ReceiveMsg);
                newThread.IsBackground = true;
                newThread.Start(aSocket);
            }
        }

        /********************************************************** 界面的操作，与Socket无关 ********/

        private void btn_send_Click(object sender, EventArgs e) {
            String msg = this.getMsgContent();
            if (msg.Length == 0) {
                appendToHistory("请输入内容\n");
                return;
            }

            List<string> failed = this.sendMsg2OnlineCli(msg);
            foreach (string error in failed) {
                appendToHistory("sendCli异常：" + error);
            }

            //发消息给客户端
            try {
                this.clientSsend(msg);
                appendToHistory("本机:\n" + msg + "\n");
            }
            catch (Exception ex) {
                appendToHistory("sendServer异常:" + ex.Message);
                if (getClientS() == null) {
                    return;
                }
                try {
                    this.closeClientS();
                }
                catch (Exception excep) {
                    appendToHistory("close异常" + excep.Message);
                }
            }

            tb_msg.Text = "";
        }


        private void btn_conn_Click(object sender, EventArgs e) {
            IPEndPoint ipPoint = this.getServerAdd();
            if (this.isclientSConnected()) {
                tb_history.AppendText("已连接 \n");
                return;
            }

            try {
                this.conn(ipPoint);
                appendToHistory("服务器连接成功\n");

                //开新线程 - 接受信息
                Thread newThread = new Thread(ReceiveServerMsg_thread);
                newThread.IsBackground = true;
                newThread.Start();

                Thread listenThread = new Thread(startListen_thread);
                listenThread.IsBackground = true;
                listenThread.Start();

            }
            catch (Exception ex) {
                appendToHistory("连接失败\n失败原因：" + ex.Message.ToString());
            }
        }

        /**
         * 接受服务端消息的线程
         */
        public void ReceiveServerMsg_thread() {
            while (true) {
                try {
                    MsgData md = this.receiveServerMsgData();
                    if (md.cliList.Length > 0) {
                        showOnline(md.cliList);
                    }
                    if (md.msg.Length > 0) {
                        appendToHistory("服务端：\n" + md.msg + "\n");
                    }
                }
                catch (Exception ex) {
                    appendToHistory("服务端 - Receive异常：\n" + ex.Message + "\n");
                    //appendToHistory("断开与" + clientS.RemoteEndPoint.ToString() + "连接\n");

                    this.closeClientS();
                    break;
                }
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
        * 获得发送内容
        */
        public String getMsgContent() {
            String msg = tb_msg.Text;

            return msg;
        }

        /********************************************************** 和界面的操作，与Socket无关 ********/


        /*********************************************************** UI修改函数 *******/
        /*********************************************************** 下面一般无关联 *******/

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

        /*********************************************************** UI修改函数 *******/

        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("");
        }
    }
}







