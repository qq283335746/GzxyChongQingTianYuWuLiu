using System.Reflection;
using System.Configuration;
using TygaSoft.ICacheDependency;

namespace TygaSoft.CacheDependencyFactory
{
    public static class DependencyAccess
    {
        private static IMsSqlCacheDependency LoadInstance(string className)
        {
            string[] paths = ConfigurationManager.AppSettings["CacheDependencyAssembly"].Split(',');
            string fullyQualifiedClass = paths[0] + "." + className;

            return (IMsSqlCacheDependency)Assembly.Load(paths[1]).CreateInstance(fullyQualifiedClass);
        }

        public static IMsSqlCacheDependency CreateContentDetailDependency()
        {
            return LoadInstance("ContentDetail");
        }

        public static IMsSqlCacheDependency CreateContentTypeDependency()
        {
            return LoadInstance("ContentType");
        }

        public static IMsSqlCacheDependency CreateCategoryDependency()
        {
            return LoadInstance("Category");
        }

        public static IMsSqlCacheDependency CreateSysEnumDependency()
        {
            return LoadInstance("SysEnum");
        }
    }
}
