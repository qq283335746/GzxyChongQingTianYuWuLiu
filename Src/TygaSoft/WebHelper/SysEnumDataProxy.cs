using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using System.Data.SqlClient;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.CacheDependencyFactory;

namespace TygaSoft.WebHelper
{
    public class SysEnumDataProxy
    {
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);
        private static readonly int sysEnumTimeout = int.Parse(ConfigurationManager.AppSettings["SysEnumCacheDuration"]);

        public static List<SysEnumInfo> GetList()
        {
            SysEnum bll = new SysEnum();

            if (!enableCaching)
            {
                return bll.GetList();
            }

            string key = "sysEnum_list";
            List<SysEnumInfo> data = (List<SysEnumInfo>)HttpRuntime.Cache[key];

            if (data == null)
            {
                data = bll.GetList();

                AggregateCacheDependency cd = DependencyFacade.GetSysEnumDependency();
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(sysEnumTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }

        public static List<SysEnumInfo> GetList(string parentName)
        {
            SysEnum bll = new SysEnum();

            SqlParameter parm = new SqlParameter("@EnumName", parentName);

            if (!enableCaching)
            {
                return bll.GetList(1, 100000, "and t2.EnumName = @EnumName",parm);
            }

            string key = "sysEnum_list_" + parentName + "";
            List<SysEnumInfo> data = (List<SysEnumInfo>)HttpRuntime.Cache[key];

            if (data == null)
            {
                data = bll.GetList(1, 100000, "and t2.EnumName = @EnumName", parm);

                AggregateCacheDependency cd = DependencyFacade.GetSysEnumDependency();
                HttpRuntime.Cache.Add(key, data, cd, DateTime.Now.AddHours(sysEnumTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return data;
        }
    }
}
