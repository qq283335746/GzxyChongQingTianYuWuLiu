using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.CacheDependencyFactory;

namespace TygaSoft.WebHelper
{
    public class ContentDetailDataProxy
    {
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);
        private static readonly int contentDetailTimeout = int.Parse(ConfigurationManager.AppSettings["ContentCacheDuration"]);

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <returns></returns>
        public static List<ContentDetailInfo> GetList(int pageIndex, int pageSize, out int totalCount,string contentType)
        {
            totalCount = 0;
            if (string.IsNullOrEmpty(contentType)) throw new ArgumentException("参数值不能为空",contentType);

            ContentDetail bll = new ContentDetail();

            SqlParameter parm = new SqlParameter("@TypeCode", SqlDbType.VarChar, 36);
            parm.Value = contentType;

            if (!enableCaching)
                return bll.GetList(pageIndex, pageSize, out totalCount, "and ct.TypeCode = @TypeCode",parm);

            string key = "contentDetails_"+contentType+"";
            string keyCount = "contentDetailCount_" + contentType + "";
            List<ContentDetailInfo> data = (List<ContentDetailInfo>)HttpRuntime.Cache[key];
            if (HttpRuntime.Cache[keyCount] != null) totalCount = (Int32)HttpRuntime.Cache[key]; 

            if (data == null)
            {
                data = bll.GetList(pageIndex, pageSize, out totalCount, "and ct.TypeCode = @TypeCode", parm);

                AggregateCacheDependency cd = DependencyFacade.GetContentDetailDependency();
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(contentDetailTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                HttpRuntime.Cache.Add(keyCount, totalCount, cd, DateTime.Now.AddHours(contentDetailTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// 获取当前ID对应数据
        /// </summary>
        /// <param name="nId"></param>
        /// <returns></returns>
        public static ContentDetailInfo GetModel(object nId)
        {
            ContentDetail bll = new ContentDetail();

            if (!enableCaching)
                return bll.GetModel(nId);

            string key = "contentDetail_" + nId.ToString() + "";
            ContentDetailInfo data = (ContentDetailInfo)HttpRuntime.Cache[key];

            if (data == null)
            {
                data = bll.GetModel(nId);

                AggregateCacheDependency cd = DependencyFacade.GetContentDetailDependency();
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(contentDetailTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        /// <summary>
        /// 获取当前内容类型对应数据
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ContentDetailInfo GetModel(string contentType)
        {
            ContentDetail bll = new ContentDetail();
            SqlParameter parm = new SqlParameter("@TypeCode", SqlDbType.NVarChar, 50);
            parm.Value = contentType;

            if (!enableCaching)
            {
                return bll.GetList("and ct.TypeCode = @TypeCode ", parm).FirstOrDefault();
            }

            string key = "contentDetail_single_" + contentType + "";
            ContentDetailInfo data = (ContentDetailInfo)HttpRuntime.Cache[key];

            if (data == null)
            {
                data = bll.GetList("and ct.TypeCode = @TypeCode ", parm).FirstOrDefault();

                AggregateCacheDependency cd = DependencyFacade.GetContentDetailDependency();
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(contentDetailTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }
    }
}
