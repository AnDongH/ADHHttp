using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.DAO {
    public class DaoGdbUserAccount {
        public long uid { get; set; }
        public string hashed_pw { get; set; }
        public string salt_value { get; set; }
    }

}
