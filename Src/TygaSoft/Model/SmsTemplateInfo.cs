using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class SmsTemplateInfo
    {
        public object Id { get; set; }

        public object UserId { get; set; }

        public string Title { get; set; }

        public string ParamsCode { get; set; }

        public string ParamsName { get; set; }

        public string ParamsValue { get; set; }

        public string SmsContent { get; set; }

        public string TemplateType { get; set; }

        public bool IsDefault { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        #region 业务定义

        public string OrderCode { get; set; }

        #endregion
    }
}
