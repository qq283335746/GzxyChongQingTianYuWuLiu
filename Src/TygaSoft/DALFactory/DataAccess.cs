using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using TygaSoft.IDAL;

namespace TygaSoft.DALFactory
{
    public sealed class DataAccess
    {
        private static readonly string[] paths = ConfigurationManager.AppSettings["WebDALMsSqlProvider"].Split(',');

        public static ISmsSend CreateSmsSend()
        {
            string className = paths[0] + ".SmsSend";
            return (ISmsSend)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static ISmsTemplate CreateSmsTemplate()
        {
            string className = paths[0] + ".SmsTemplate";
            return (ISmsTemplate)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static ITyUser CreateTyUser()
        {
            string className = paths[0] + ".TyUser";
            return (ITyUser)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IOrder CreateOrder()
        {
            string className = paths[0] + ".Order";
            return (IOrder)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IPDAOrderScan CreatePDAOrderScan()
        {
            string className = paths[0] + ".PDAOrderScan";
            return (IPDAOrderScan)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IPDAOrderScanDetail CreatePDAOrderScanDetail()
        {
            string className = paths[0] + ".PDAOrderScanDetail";
            return (IPDAOrderScanDetail)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IRole CreateRole()
        {
            string className = paths[0] + ".Role";
            return (IRole)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static ICategory CreateCategory()
        {
            string className = paths[0] + ".Category";
            return (ICategory)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IContentType CreateContentType()
        {
            string className = paths[0] + ".ContentType";
            return (IContentType)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IContentDetail CreateContentDetail()
        {
            string className = paths[0] + ".ContentDetail";
            return (IContentDetail)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static ISysEnum CreateSysEnum()
        {
            string className = paths[0] + ".SysEnum";
            return (ISysEnum)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static ISitePoint CreateSitePoint()
        {
            string className = paths[0] + ".SitePoint";
            return (ISitePoint)Assembly.Load(paths[1]).CreateInstance(className);
        }

        public static IReportContent CreateReportContent()
        {
            string className = paths[0] + ".ReportContent";
            return (IReportContent)Assembly.Load(paths[1]).CreateInstance(className);
        }
    }
}
