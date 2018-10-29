using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.TableCacheDependency
{
    public class ContentType : MsSqlCacheDependency
    {
        public ContentType() : base("ContentTableDependency") { }
    }
}
