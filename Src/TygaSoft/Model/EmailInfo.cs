using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class EmailInfo
    {
        public string FromEmail { get; set; }

        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public string UseByType { get; set; }

        public string TemplatePath { get; set; }

        public object[] Parms { get; set; }
    }
}
