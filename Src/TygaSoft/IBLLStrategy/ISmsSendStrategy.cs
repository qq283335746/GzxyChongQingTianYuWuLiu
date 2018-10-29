using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.Model;

namespace TygaSoft.IBLLStrategy
{
    public interface ISmsSendStrategy
    {
        void Insert(SmsSendInfo model);
    }
}
