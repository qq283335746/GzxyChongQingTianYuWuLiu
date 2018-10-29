using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace TygaSoft.MailUtility
{
    public class MailHelper
    {
        public SmtpClient client;

        public MailHelper()
        {
            if (client == null) client = new SmtpClient();
        }

        public void Send(string to, string subject, string templatePath,params object[] args)
        {
            MailMessage message = new MailMessage();
            MailAddress toAddress = new MailAddress(to);
            message.To.Add(toAddress);
            message.Subject = subject;
            message.Body = string.Format(File.ReadAllText(templatePath), args);
            message.IsBodyHtml = true;

            Send(message);
        }

        public void Send(MailMessage message)
        {
            try
            {
                client.Send(message);
            }
            catch
            { }
            finally
            {
                message.To.Clear();
                SetMailMessageDispose(message);
            }
        }

        public void SetSmtpClientDispose()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        public void SetMailMessageDispose(MailMessage message)
        {
            if (message != null)
            {
                message.Dispose();
                message = null;
            }
        }

    }
}
