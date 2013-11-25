using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace socketClient_win {
    class Socket_Listen {

        public Socket listenS;
        public StreamReader streamR;
        public StreamWriter streamW;
        private int maxCount = 10;
        public Form1 f1;
        private int bytLength = 1000;

        /**
         * 开始监听
         */
        public void startListen(IPAddress localIp, int localPort) {
            IPEndPoint ipPoint = new IPEndPoint(localIp, localPort);

            listenS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenS.Bind(ipPoint);
            listenS.Listen(maxCount);
        }

        /**
        * 监听连接
        */
        public void listenConn() {
            while (true) {
                Socket aSocket = listenS.Accept();
                f1.appendToHistory("连接来自[" + aSocket.RemoteEndPoint.ToString() + "]\n");

                Thread newThread = new Thread(ReceiveMsg);
                newThread.IsBackground = true;
                newThread.Start(aSocket);
            }
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

                    f1.appendToHistory("来自" + aSocket.RemoteEndPoint.ToString() + "\n" + resString + "\n");
                }
                catch (Exception ex) {
                    f1.appendToHistory("receive异常：" + ex.Message + "\n");
                    f1.appendToHistory("断开与" + aSocket.RemoteEndPoint.ToString() + "连接\n");
                    aSocket.Shutdown(SocketShutdown.Both);
                    aSocket.Close();
                    break;
                }
            }
        }

    }
}
