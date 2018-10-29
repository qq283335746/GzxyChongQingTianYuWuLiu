using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.Model;

namespace TygaSoft.IMessaging
{
    public interface ISmsSend
    {
        SmsSendInfo Receive();

        SmsSendInfo Receive(int timeout);

        void Send(SmsSendInfo model);
    }
}
