using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class SmsSendInfo
    {
        public object Id { get; set; }

        public object UserId { get; set; }

        public string OrderCode { get; set; }

        public string TranNode { get; set; }

        public string Receiver { get; set; }

        public string MobilePhone { get; set; }

        public string Customer { get; set; }

        public string SmsContent { get; set; }

        public DateTime SendDate { get; set; }

        public string CarScanCode { get; set; }

        /// <summary>
        /// 待发送、已发送、发送失败
        /// </summary>
        public short SendStatus { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        #region 业务定义

        /// <summary>
        /// 发送状态：待发送、已发送、发送失败
        /// </summary>
        public string SendStatusText { get; set; }

        /// <summary>
        /// TranNode对应的文本
        /// </summary>
        public string TranNodeText { get; set; }

        /// <summary>
        /// 号码类型(手机号、订单号、派车单号)
        /// </summary>
        public string NumberType { get; set; }

        /// <summary>
        /// 号码(手机号、订单号、派车单号)
        /// </summary>
        public string NumberCode { get; set; }

        /// <summary>
        /// 已选的模板ID
        /// </summary>
        public object SmsTemplateId { get; set; }

        /// <summary>
        /// 参数代码集
        /// </summary>
        public string ParamsCode { get; set; }

        /// <summary>
        /// 参数名称集
        /// </summary>
        public string ParamsName { get; set; }

        /// <summary>
        /// 参数值集
        /// </summary>
        public string ParamsValue { get; set; }

        /// <summary>
        /// 模板（auto(自动)、custom(自定义)）
        /// </summary>
        public string TemplateType { get; set; }

        #endregion
    }
}
