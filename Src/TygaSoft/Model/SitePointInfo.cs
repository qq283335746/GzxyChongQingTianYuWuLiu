using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class SitePointInfo
    {
        public object Id { get; set; }

        public string PointName { get; set; }

        public decimal PointNum { get; set; }

        public string Remark { get; set; }
    }
}
