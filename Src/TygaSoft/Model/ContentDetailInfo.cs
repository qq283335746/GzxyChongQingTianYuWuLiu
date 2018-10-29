using System;

namespace TygaSoft.Model
{
    [Serializable]
    public class ContentDetailInfo
    {
        #region 成员方法

        public object Id { get; set; }
        public object ContentTypeId { get; set; }
        public string Title { get; set; }
        public string ContentText { get; set; }
        public int Sort { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public object UserId { get; set; }

        #endregion
    }
}