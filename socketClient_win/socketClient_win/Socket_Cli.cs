using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace socketClient_win {
    class Socket_Cli {

        public Socket so;
        public StreamReader streamR;
        public StreamWriter streamW;

        static JavaScriptSerializer Json_static;

        /**
         * 静态构造函数
         */ 
        static Socket_Cli(){
            Json_static = new JavaScriptSerializer();
            Json_static.MaxJsonLength = 20971520;
        }

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
        * 关闭和服务器的Socket
        */
        public void closeSocket() {
            so.Shutdown(SocketShutdown.Both);
            so.Close();
        }

        /**
         * 客户端发送消息
         */ 
        public void send2Server(MsgData md) {
            String mdString = MsgData.SerializeMsg(md);
            streamW.WriteLine(mdString);
            streamW.Flush();

        }

        /**
         * 接受服务端信息 -- StreamReader
         */
        public MsgData receiveServerMsgData() {
            string resultStr = streamR.ReadLine();
            MsgData md = MsgData.DeserializeMsg(resultStr);

            return md;
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
            if (so == null)
                return;

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

        /**
         * 关闭资源
         */
        public void closeResourse() {
            if (streamR != null) {
                streamR.Close();
            }
            if (streamW != null) {
                streamW.Close();
            }
            if (so != null) {
                so.Close();
            }
        }

    }
}
