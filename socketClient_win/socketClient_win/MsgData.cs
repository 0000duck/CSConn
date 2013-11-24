using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketClient_win {
    public class MsgData {

        public String msg;
        public String cliList;

        public MsgData(String msg = "", String cliList = "") {
            this.msg = msg;
            this.cliList = cliList;
        }

        public MsgData() : this("", "") { }

    }
}
