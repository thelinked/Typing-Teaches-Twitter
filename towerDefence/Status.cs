using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace towerDefence
{
    [DataContract]
    public class Status
    {
        public DateTimeOffset created_at_dt;
        [DataMember]
        public string created_at
        {
            get { return created_at_dt.ToString("ddd MMM dd HH:mm:ss zzz yyyy"); }
            set
            {
                created_at_dt = DateTimeOffset.ParseExact(value, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
            }
        }
        [DataMember]
        public string id;
        [DataMember]
        public string text;
        [DataMember]
        public string source;
        [DataMember]
        public string truncated;
        [DataMember]
        public string in_reply_to_status_id;
        [DataMember]
        public string in_reply_to_user_id;
        [DataMember]
        public string favorited;
        [DataMember]
        public string in_reply_to_screen_name;
        [DataMember]
        public User user;
        [DataMember]
        public geo geo;
        [DataMember]
        public string contributors;
    }

    [DataContract]
    public class geo
    {
        [DataMember]
        public string type;
        [DataMember]
        public string[] coordinates;
    }
}
