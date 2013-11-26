using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace socketClient_win {
    class Socket_Listen {

        public Socket listenS;
        private int maxCount = 10;
        public Form1 f1;

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
                newThread.SetApartmentState(ApartmentState.STA);
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
                    NetworkStream ns = new NetworkStream(aSocket);
                    StreamReader sr = new StreamReader(ns);
                    string resultStr = sr.ReadLine();
                    MsgData md = MsgData.DeserializeMsg(resultStr);
                    string ipAndPort = aSocket.RemoteEndPoint.ToString();

                    switch (md.type) { 
                        case "TEXT":
                            this.showMsg(md, ipAndPort);
                            break;
                        case "FILE":
                            Boolean accecptable = this.isFileAccept(md);
                            if (!accecptable)
                                continue;

                            string folderP = FileTranser.getFolderPath();
                            if (folderP.Length == 0)
                                continue;

                            this.WriteFile(md, folderP);
                            break;
                    }
                }
                catch (Exception ex) {
                    f1.appendToHistory("receive异常：" + ex.Message + " - ");
                    f1.appendToHistory("断开与" + aSocket.RemoteEndPoint.ToString() + "连接\n");
                    aSocket.Shutdown(SocketShutdown.Both);
                    aSocket.Close();
                    break;
                }
            }
        }

        /**
         * 是否接受文件
         */
        public Boolean isFileAccept(MsgData md) {
            String msg = "传来文件" + md.fileName + ", 文件大小"+ md.fileSize + "KB\n是否接受？";
            string title = "文件传输";
            if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
                return true;
            }
            return false;
            
        }

        /**
         * 写入文件
         */
        public void WriteFile(MsgData md, String localFolder) {
             String fileName = localFolder + @"\" + md.fileName;
             StreamWriter sw = null;
             try {
                 sw = new StreamWriter(fileName);
                 sw.WriteLine(md.msg);
             }
             catch (Exception ex) {
                 f1.appendToHistory("写文件异常：" + ex.Message);
             }
             finally {
                 if (sw != null)
                     sw.Close();
             }
        
        }

        /**
         * 显示信息
         */
        public void showMsg(MsgData md, String remoteip) {
            string msg = md.msg;
            Debug.WriteLine("");

            f1.appendToHistory("来自" + remoteip + "\n" + msg + "\n");
        }

        /**
         * 读取文件内容
         */
        public String readerFile(MsgData md) {
            String fileName = md.fileName;
            string str = "";
            using (StreamReader sr = new StreamReader(fileName)) {
                str = sr.ReadLine();
            }
            return str;
        }



    }
}
