using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class PDAOrderScanDetailInfo
    {
        public object Id { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public object CurrNodeId { get; set; }

        /// <summary>
        /// 扫描时间
        /// </summary>
        public DateTime ScanTime { get; set; }

        /// <summary>
        /// 在途描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public object UserId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }

        #region 其它

        public string SysEnumValue { get; set; }

        public string SysEnumRemark { get; set; }

        public int Sort { get; set; }

        public string UserName { get; set; }

        #endregion
    }
}
