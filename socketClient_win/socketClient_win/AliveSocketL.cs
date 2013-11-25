using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace socketClient_win {
    class SocketList {

        private List<Socket> alive_sockets = new List<Socket>();


        /**
        * 检测ipAndPort 在链接中的socket是否已存在
        */
        public int checkConnSocket(String ipAndPort) {
            for (int i = 0; i < alive_sockets.Count; i++) {
                String soIpAndP = alive_sockets[i].RemoteEndPoint.ToString();
                if (soIpAndP == ipAndPort) {
                    return i;
                }
            }
            return -1;
        }

        /**
        * 发送信息给在线客户端  
        * 返回 List<string> failedList
        */
        public string sendMsg(String msg, String ipAndPort) {
            String error = "";

            //获得Socket so
            int index = this.checkConnSocket(ipAndPort);
            Socket so = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (index == -1) {
                IPEndPoint ipPort = CliSocket.translateIpEndPoint(ipAndPort);
                so.Connect(ipPort);
                alive_sockets.Add(so);
            }
            else {
                so = alive_sockets[index];
            }

            //发送
            try {
                so.Send(Encoding.UTF8.GetBytes(msg));
            }
            catch (Exception ex) {
                error = ipAndPort + "发送失败\n" + ex.Message;
                CliSocket.closeTheClose(so);
            }

            return error;
        }


        /**
        * 发送信息给在线客户端  
        * 返回 List<string> failedList
        */
        public List<string> sendMsg2SockeList(String msg, List<String> target) {
            List<string> failedList = new List<string>();

            foreach (String ipAndPort in target) {
                string error = this.sendMsg(msg, ipAndPort);
                if (error.Length > 0) {
                    failedList.Add(error);
                }
            }
            return failedList;
        }

    }
}
