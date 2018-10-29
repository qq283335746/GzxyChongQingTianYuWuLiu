using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.IBLLStrategy;
using TygaSoft.MessagingFactory;
using TygaSoft.Model;
using TygaSoft.IMessaging;

namespace TygaSoft.BLL
{
    public class SmsAsynchronous : ISmsSendStrategy
    {
        private static readonly ISmsSend asynchDal = QueueAccess.CreateSmsSend();

        public void Insert(SmsSendInfo model)
        {
            asynchDal.Send(model);
        }
    }
}
