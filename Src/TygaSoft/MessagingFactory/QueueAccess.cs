using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using TygaSoft.IMessaging;

namespace TygaSoft.MessagingFactory
{
    public sealed class QueueAccess
    {
        private static readonly string[] path = ConfigurationManager.AppSettings["MsmqMessaging"].Split(new char[] { ',' });

        private QueueAccess() { }

        public static ISmsSend CreateSmsSend()
        {
            string className = path[0] + ".SmsSend";
            return (ISmsSend)Assembly.Load(path[1]).CreateInstance(className);
        }

        public static IEmail CreateEmail()
        {
            string className = path[0] + ".Email";
            return (IEmail)Assembly.Load(path[1]).CreateInstance(className);
        }
    }
}
