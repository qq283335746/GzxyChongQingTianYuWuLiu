using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Messaging;
using TygaSoft.IMessaging;
using TygaSoft.Model;

namespace TygaSoft.MsmqMessaging
{
    public class SmsSend : TygaSoftQueue, ISmsSend
    {
        private static readonly string queuePath = ConfigurationManager.AppSettings["SmsMsmqPath"];
        private static int queueTimeout = 20;

        public SmsSend() : base(queuePath, queueTimeout)
        {
            queue.Formatter = new BinaryMessageFormatter();
        }

        public new SmsSendInfo Receive()
        {
            base.transactionType = MessageQueueTransactionType.Automatic;
            return (SmsSendInfo)((Message)base.Receive()).Body;
        }

        public SmsSendInfo Receive(int timeout)
        {
            base.timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeout));
            return Receive();
        }

        public void Send(SmsSendInfo model)
        {
            base.transactionType = MessageQueueTransactionType.Single;
            base.Send(model);
        }
    }
}
