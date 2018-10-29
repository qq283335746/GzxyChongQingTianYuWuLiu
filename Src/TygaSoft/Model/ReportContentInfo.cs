using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class ReportContentInfo
    {
        public object Id { get; set; }

        public object UserId { get; set; }

        public string FromUrl { get; set; }

        public string FromType { get; set; }

        public string GiveName { get; set; }

        public DateTime FromDate { get; set; }
    }
}
