using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace TygaSoft.DALFactory
{
    public class XmlDataAccess
    {
        private static readonly string[] path = ConfigurationManager.AppSettings["WebDALXmlProvider"].Split(',');
    }
}
