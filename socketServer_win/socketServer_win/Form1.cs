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

namespace socketServer_win {
    public partial class Form1 : Form {

        private Socket serverS;
        private int maxCliCount = 10;
        private int byteLength = 1000;
        private List<Socket> clientList = new List<Socket>();
        private List<Socket> cliList_live;

        private int testcount = 0;

        public Form1() {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) {
            String ipStr = this.getIP();
            int port = this.getPort();
            this.startServer(ipStr, port);
        }
        
        /**
         * 获得ip 
         */
        public string getIP() {
            string ip = tb_ip.Text;

            return ip;
        }

        /**
         * 启动服务
         */
        public void startServer(string ipStr, int port) {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipPoint = new IPEndPoint(ip, port);
            if (serverS != null) {
                appendToHistory("已启动监听\n");
                return;
            }

            serverS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverS.Bind(ipPoint);
            serverS.Listen(maxCliCount);
            appendToHistory("启动监听" + serverS.LocalEndPoint.ToString() + "\n");

            Thread newThread = new Thread(listenClientConn);
            newThread.Name = "listennnnnn";
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
                cliList_live = this.getConnctedCli();
                this.showCliList(cliList_live);
                this.sendOnlineListToCli(cliList_live);

                appendToHistory("连接来自" + aSocket.RemoteEndPoint.ToString() + "\n");

                MsgData md = new MsgData("这里是服务端");
                this.sendMsgData(md, aSocket);

                Thread newThread = new Thread(RecieveMsg);
                newThread.Name = "receieveeeee";
                newThread.IsBackground = true;
                newThread.Start(aSocket);
            }
        }



        /**
         * 发送在线客户端地址给，所有在线客户端
         */
        public void sendOnlineListToCli(List<Socket> cliList) {
            String cliString = "";
            foreach (Socket so in cliList) {
                cliString = cliString + so.RemoteEndPoint.ToString() + "&";
            }
            if (cliString.Length > 0) {
                cliString = cliString.Substring(0, cliString.Length - 1);
            }
            MsgData md = new MsgData("", cliString);

            this.sendMsgData(md, cliList);
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

        /*************************************************************************** Socket 发送消息 **********/


        /**
         * 接受信息
         */
        public void RecieveMsg(Object obj) {
            Socket aSocket = (Socket)obj;
            while (true) {
                try {
                    Byte[] res = new Byte[byteLength];
                    int length = aSocket.Receive(res);

                    this.preventAlwaysLoop();

                    String resString = Encoding.UTF8.GetString(res, 0, length);
                    appendToHistory("Msg -来自" + aSocket.RemoteEndPoint.ToString() + "\n" + resString + "\n");
                }
                catch (Exception ex) {
                    appendToHistory("异常：" + ex.Message + "\n");
                    this.closeTheSocket(aSocket);

                    cliList_live = this.getConnctedCli();
                    this.showCliList(cliList_live);
                    this.sendOnlineListToCli(cliList_live);
                    break;
                }
            }
        }

        /**
         * 阻止无限循环
         */
        public void preventAlwaysLoop() {
            testcount++;
            if (testcount > 100)
            {
                testcount = 0;
                throw new Exception("循环次数太多!");
            }
        }

        /**
         * 关闭指定 Socket
         */
        public void closeTheSocket(Socket so) {
            so.Shutdown(SocketShutdown.Both);
            so.Close();
        }

        /*************************************************************************** END Socket 发送消息 **********/


        /*************************************************************************** Socket 发送消息 **********/
        //////只有服务的发送， 客户端的接收才用 MsgData 对象的数据格式
        ////// 其他时候， 客户端-》服务端， 客户端《-》客户端都直接发送字符串

        /**
         * 发送消息 - MsgData
         */
        public void sendMsgData(MsgData md, Socket so) {
            JavaScriptSerializer json = new JavaScriptSerializer();
            String mdString = json.Serialize(md);

            // 待测试 sw.WriteLine(Object obj); 是否可以直接传 Object
            //如果可以，传 Object ，不用转Json 格式
            NetworkStream ns = new NetworkStream(so);
            StreamWriter sw = new StreamWriter(ns);
            
            sw.WriteLine(mdString);
            sw.Flush();

        }

        /**
         * 发送消息 - MsgData
         */
        public void sendMsgData(MsgData md, List<Socket> sendList) {
            foreach (Socket so in sendList) {
                sendMsgData(md, so);
            }
        }
        /*************************************************************************** END Socket 发送消息 **********/


        /**************************************************************************** 按钮 *************/

        private void btn_send_Click(object sender, EventArgs e)
        {
            String msg = getMsgContent();
            List<Socket> sendList = this.getSendTarget();
            if (sendList.Count == 0)
            {
                appendToHistory("请选择发送对象\n");
                return;
            }
            MsgData md = new MsgData(msg);
            this.sendMsgData(md, sendList);

            //显示发送信息
            appendToHistory("服务端 - 发往");
            foreach (Socket so in sendList)
            {
                appendToHistory(so.RemoteEndPoint.ToString() + ", ");
            }
            appendToHistory("\n" + msg + "\n");
            tb_msg.Clear();
        }
        
        /**************************************************************************** 按钮 *************/




        /*************************************************************************** UI函数  ***********/
        /**********************************************************************  一般与Socket无关 *******/



        /**
         * 显示连接状态的客户端
         */
        public void showCliList(List<Socket> cliList)
        {
            clearClientList_lb();

            for (int i = 0; i < cliList.Count; i++)
            {
                String cliAddress = cliList[i].RemoteEndPoint.ToString();
                String cliInfo = i + ". " + cliAddress;
                addToClientList_lb(cliInfo);
            }
        }

        private delegate void InvokeCallback(String msg);
        /**
         * 给历史记录增加文字
         */
        public void appendToHistory(String msg)
        {
            if (tb_history.InvokeRequired)
            {
                InvokeCallback callback = new InvokeCallback(appendToHistory);
                tb_history.Invoke(callback, msg);
            }
            else
            {
                if (tb_history == null)
                {
                    return;
                }
                tb_history.AppendText(msg);
            }
        }

        private delegate void addListInvokeCallback(String msg);
        /**
         * 增加客户端列表 ListBox
         */
        public void addToClientList_lb(String msg)
        {
            if (checked_lb_client.InvokeRequired)
            {
                addListInvokeCallback callback = new addListInvokeCallback(addToClientList_lb);
                checked_lb_client.Invoke(callback, msg);
            }
            else
            {
                checked_lb_client.Items.Add(msg);
            }
        }

        private delegate void listInvokeCallback();
        /**
         * 清空客户端列表 ListBox
         */
        public void clearClientList_lb()
        {
            if (checked_lb_client.InvokeRequired)
            {
                listInvokeCallback callback = new listInvokeCallback(clearClientList_lb);
                checked_lb_client.Invoke(callback);
            }
            else
            {
                checked_lb_client.Items.Clear();
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
        * 获得端口号
        */
        public int getPort()
        {
            String portStr = tb_port.Text;
            int port = int.Parse(portStr);

            return port;
        }

        /**
         * 获取发送内容
         */
        public String getMsgContent() {
            String msg = tb_msg.Text;

            return msg;
        }

        private void tb_history_TextChanged(object sender, EventArgs e){
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("");
        }
    }
}
