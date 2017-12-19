using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Narsha.Models
{
    public class MemberElement
    {
        [DataContract]
        public class RootObject
        {
            [DataMember]
            public string Code { get; set; }

            [DataMember]
            public string Employee_id { get; set; }

            [DataMember]
            public string Employee_name { get; set; }

            [DataMember]
            public string Employee_gender { get; set; }

            [DataMember]
            public int Employee_age { get; set; }

            [DataMember]
            public string Employee_profile { get; set; }

            [DataMember]
            public string Employee_date { get; set; }

            [DataMember]
            public string employee_department { get; set; }
        }
    }
}
