using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.IBLLStrategy;
using TygaSoft.DALFactory;
using TygaSoft.IDAL;
using TygaSoft.Model;

namespace TygaSoft.BLL
{
    public class SmsSynchronous : ISmsSendStrategy
    {
        private static readonly ISmsSend dal = DataAccess.CreateSmsSend();

        public void Insert(SmsSendInfo model)
        {
            dal.Insert(model);
        }
    }
}
