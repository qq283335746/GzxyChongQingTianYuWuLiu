﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.Model
{
    [Serializable]
    public class WebSitemapInfo
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Roles { get; set; }
    }
}
