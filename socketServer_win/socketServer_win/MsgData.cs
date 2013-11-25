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

    }
}
