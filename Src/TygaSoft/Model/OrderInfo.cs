using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    public class OrderInfo
    {
        public string OrderCode { get; set; }

        public DateTime BusinessDate { get; set; }

        public float TotalPackageCount { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }
    }
}
