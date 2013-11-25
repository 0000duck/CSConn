using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace socketServer_win {
    public class MsgData {

        public String msg;
        public String cliList;
        public String userName;

        public MsgData(String msg = "", String cliList="", String userName="") {
            this.msg = msg;
            this.cliList = cliList;
            this.userName = userName;
        }

        public MsgData() : this("", "", "") {
        }

        /**
         * 反序列数据
         */
        public static MsgData DeserializeMsg(String mdString) {
            JavaScriptSerializer json = new JavaScriptSerializer();
            MsgData md = json.Deserialize<MsgData>(mdString);

            return md;
        }

        /**
         * 序列化为Jison
         */
        public static String SerializeMsg(MsgData md) {
            JavaScriptSerializer json = new JavaScriptSerializer();
            string mdString = json.Serialize(md);

            return mdString;
        }


    }
}
