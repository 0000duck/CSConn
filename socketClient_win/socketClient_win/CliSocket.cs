using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace socketClient_win {
    class CliSocket {

        public Socket so;
        public StreamReader streamR;
        public StreamWriter streamW;

        /**
        * clientS 是否已连接
        */
        public Boolean isSocketConnected() {
            if (so != null && so.Connected == true) {
                return true;
            }
            return false;
        }

        /**
        * 连接服务器 
        */
        public void conn(IPEndPoint ipPoint) {
            so = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            so.Connect(ipPoint);

            NetworkStream ns = new NetworkStream(so);
            streamR = new StreamReader(ns);
            streamW = new StreamWriter(ns);

        }

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
        * 关闭和服务器的Socket
        */
        public void closeSocket() {
            so.Shutdown(SocketShutdown.Both);
            so.Close();
        }

        /**
         * 客户端发送消息
         */ 
        public void send2Server(string msg) {
            so.Send(Encoding.UTF8.GetBytes(msg));
        }

        /**
        * 获得和服务器连接的端口
        */
        public int getLocalPort() {
            String ipAndPort = so.LocalEndPoint.ToString();
            String[] info = ipAndPort.Split(':');
            String portStr = info[1];
            int port = int.Parse(portStr);

            return port;
        }


        /**
        * 关闭指定的Socket
        */
        public static void closeTheClose(Socket so) {
            so.Shutdown(SocketShutdown.Both);
            so.Close();
        }

        /**
        * String 转换为 IPEndPoint
        */
        public static IPEndPoint translateIpEndPoint(String ipAndPort) {
            String[] info = ipAndPort.Split(':');
            String ipStr = info[0];
            String portStr = info[1];
            int port = int.Parse(portStr);

            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipPort = new IPEndPoint(ip, port);

            return ipPort;
        }


    }
}
