using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Messaging;
using TygaSoft.Model;

namespace TygaSoft.MsmqMessaging
{
    public class Email : TygaSoftQueue, IMessaging.IEmail
    {
        // Path example - FormatName:DIRECT=OS:MyMachineName\Private$\OrderQueueName
        private static readonly string queuePath = ConfigurationManager.AppSettings["EmailMsmqPath"];
        private static int queueTimeout = 20;

        public Email()
            : base(queuePath, queueTimeout)
        {
            // Set the queue to use Binary formatter for smaller foot print and performance
            queue.Formatter = new BinaryMessageFormatter();
        }

        /// <summary>
        /// 接收消息队列中的待发送邮件
        /// </summary>
        /// <returns></returns>
        public new EmailInfo Receive()
        {
            // This method involves in distributed transaction and need Automatic Transaction type
            base.transactionType = MessageQueueTransactionType.Automatic;
            return (EmailInfo)((Message)base.Receive()).Body;
        }

        public EmailInfo Receive(int timeout)
        {
            base.timeout = TimeSpan.FromSeconds(Convert.ToDouble(timeout));
            return Receive();
        }

        /// <summary>
        /// 将待发送邮件发送到消息队列中
        /// </summary>
        /// <param name="model"></param>
        public void Send(EmailInfo model)
        {
            // This method does not involve in distributed transaction and optimizes performance using Single type
            base.transactionType = MessageQueueTransactionType.Single;
            base.Send(model);
        }
    }
}
