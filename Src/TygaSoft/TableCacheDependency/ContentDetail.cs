using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.TableCacheDependency
{
    public class ContentDetail : MsSqlCacheDependency
    {
        public ContentDetail() : base("ContentTableDependency") { }
    }
}
