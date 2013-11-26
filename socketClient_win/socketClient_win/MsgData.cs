using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace socketClient_win {
    public class MsgData {

        public String msg;
        public String cliList;
        public String userName;
        public Boolean isFile = false;
        public string type = "";        //type 为"TEXT"是纯文本， 
                                        //"FILE" 是文件, 
                                        //"list" 是用户列表, 
                                        //"FILE_ACK" 是确认接受文件
                                        //"FILE_STREAM"是文件流
        public string fileName = "";
        public string acceptfileName = "";
        public Boolean acceptFile = false;

        static JavaScriptSerializer Json_static;

        static MsgData() {
            Json_static = new JavaScriptSerializer();
            Json_static.MaxJsonLength = 20971520;
        }

        public MsgData(String msg = "", String cliList = "", String userName ="") {
            this.msg = msg;
            this.cliList = cliList;
            this.userName = userName;
        }

        public MsgData() : this("", "", "") { }

        /**
        * 反序列数据
        */
        public static MsgData DeserializeMsg(String mdString) {
            MsgData md = Json_static.Deserialize<MsgData>(mdString);

            Debug.WriteLine("");
            return md;
        }

        /**
         * 序列化为Jison
         */
        public static String SerializeMsg(MsgData md) {
            string mdString = Json_static.Serialize(md);

            return mdString;
        }

    }
}
