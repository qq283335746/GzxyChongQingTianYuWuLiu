using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class PDAOrderScanInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode { get; set; }

        public object NextNodeId { get; set; }

        public object CurrNodeId { get; set; }

        public bool IsFinish { get; set; }

        public DateTime LastUpdatedDate { get; set; }
    }
}
