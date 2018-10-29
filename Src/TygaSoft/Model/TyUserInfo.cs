using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class TyUserInfo
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsEnable { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public string OrganizationName { get; set; }
    }
}
