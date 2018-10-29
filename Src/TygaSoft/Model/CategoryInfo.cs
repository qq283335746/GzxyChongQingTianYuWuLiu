using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class CategoryInfo
    {
        public object Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public object ParentId { get; set; }
        public int Sort { get; set; }
        public string Remark { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
