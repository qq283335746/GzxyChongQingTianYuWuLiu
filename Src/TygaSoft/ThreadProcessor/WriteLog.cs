using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Runtime.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace TygaSoft.ThreadProcessor
{
    public class WriteLog
    {
        static string path = ConfigurationManager.AppSettings["WS_Log"].TrimEnd('\\');
        string logFile;

        public WriteLog()
        {
            DateTime currTime = DateTime.Now;
            string currPath = path + "\\" + currTime.ToString("yyyyMM");
            if (!Directory.Exists(currPath)) Directory.CreateDirectory(currPath);

            logFile = currPath + "\\" + currTime.ToString("yyyyMMdd") + ".txt";
        }

        public void Write(string s)
        {
            s = string.Format("Thread:{2}    {0}    {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), s, Thread.CurrentThread.ManagedThreadId);

            Task.Factory.StartNew(() => File.AppendAllText(logFile, s));
        } 

    }
}
