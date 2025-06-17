using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class MailInfo {

        public MailRewardInfo rewards { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int mail_id { get; set; }
        public int remain_time { get; set; }
        public bool is_received { get; set; }

        public MailInfo(MailRewardInfo rewards, string title, string content, int mail_id, int remain_time, bool is_received) {
            this.rewards = rewards;
            this.title = title;
            this.content = content;
            this.mail_id = mail_id;
            this.remain_time = remain_time;
            this.is_received = is_received;
        }
    }

}
