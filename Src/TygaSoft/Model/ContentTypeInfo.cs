using System;

namespace TygaSoft.Model
{
    [Serializable]
    public class ContentTypeInfo
    {
        #region 成员方法

        public object Id { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public object ParentId { get; set; }
        public int Sort { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        //业务需要
        public string ParentName { get; set; }

        #endregion
    }
}