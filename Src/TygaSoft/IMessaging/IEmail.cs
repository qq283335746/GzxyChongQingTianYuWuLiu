using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.Model;

namespace TygaSoft.IMessaging
{
    public interface IEmail
    {
        EmailInfo Receive();

        EmailInfo Receive(int timeout);

        void Send(EmailInfo model);
    }
}
