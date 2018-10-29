using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Configuration;
using System.Web;
using System.Web.Security;
using TygaSoft.Model;
using TygaSoft.BLL;

namespace TygaSoft.ThreadProcessor
{
    public class TyUserProcessor
    {
        static int threadCount = 1;

        public static void Processor()
        {
            Thread thread;
            Thread[] workerThreads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                thread = new Thread(new ThreadStart(WorkProcessor));
                thread.IsBackground = true;
                //thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                workerThreads[i] = thread;
            }
        }

        private static void WorkProcessor()
        {
            Random rnd = new Random();
            bool isPassNight = false; //是否已到深夜0点
            DateTime startTime = DateTime.MinValue; //开始时间
            DateTime endTime = DateTime.MinValue; //结束时间

            try
            {
                TyUser bll = new TyUser();
                SysEnum seBll = new SysEnum();

                while (true)
                {
                    if (!isPassNight)
                    {
                        if (DateTime.Now.Hour == 0)
                        {
                            isPassNight = true;
                        }
                    }

                    List<SysEnumInfo> seList = seBll.GetList("and t2.EnumCode = 'UserProcessor'");
                    double runTimeout = 0;
                    double.TryParse(seList.Find(m => m.EnumCode == "RunTimeout").EnumValue.Trim(), out runTimeout);
                    bool isOff = seList.Find(m => m.EnumCode == "On/Off").EnumValue.Trim().ToLower() == "off" ? true : false;
                    if (isOff)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    string sStartTime = seList.Find(m => m.EnumCode.Trim() == "StartTime").EnumValue;
                    string sEndTime = seList.Find(m => m.EnumCode.Trim() == "EndTime").EnumValue;

                    if ((sStartTime.IndexOf(':') != -1) && (sEndTime.IndexOf(':') != -1))
                    {
                        DateTime currTime = DateTime.Now;
                        sStartTime = string.Format("{0} {1}", currTime.ToString("yyyy-MM-dd"), sStartTime);
                        sEndTime = string.Format("{0} {1}", currTime.ToString("yyyy-MM-dd"), sEndTime);

                        DateTime.TryParse(sStartTime, out startTime);
                        DateTime.TryParse(sEndTime, out endTime);
                        DateTime maxTime = DateTime.Parse(string.Format("{0} {1}", currTime.ToString("yyyy-MM-dd"), "23:59:59"));

                        if ((startTime != DateTime.MinValue) && (endTime != DateTime.MinValue))
                        {
                            if (currTime >= endTime || currTime <= startTime)
                            {
                                Thread.Sleep(5000);
                                continue;
                            }
                        }
                    }

                    #region 同步用户表开始

                    TyUser tyuserBll = new TyUser();

                    List<string> newUsers = new List<string>();
                    string[] oldUsers = tyuserBll.GetTyUsers();
                    List<TyUserInfo> userList = tyuserBll.GetList();

                    foreach (string userName in oldUsers)
                    {
                        if (!userList.Exists(m => m.UserName == userName))
                        {
                            newUsers.Add(userName);
                        }
                    }

                    if (newUsers.Count > 0)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            foreach (string userName in newUsers)
                            {
                                string psw = (rnd.NextDouble() * Int32.MaxValue).ToString().PadLeft(6, '0').Substring(0, 6);
                                Membership.CreateUser(userName, psw, "" + userName + "@tygaweb.com");
                                Roles.AddUserToRole(userName, "Users");
                                tyuserBll.Insert(new TyUserInfo { UserName = userName, Password = psw, IsEnable = true, LastUpdatedDate = DateTime.Now });
                            }

                            scope.Complete();
                        }
                    }

                    #endregion

                    if (runTimeout > 0)
                    {
                        TimeSpan ts = DateTime.Now.AddMinutes(runTimeout) - DateTime.Now;
                        Thread.Sleep(ts);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog log = new WriteLog();
                log.Write(ex.Message);
            }
        }
    }
}
