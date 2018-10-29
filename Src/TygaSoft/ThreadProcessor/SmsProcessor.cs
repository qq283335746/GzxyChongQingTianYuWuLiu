using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using TygaSoft.Model;
using TygaSoft.BLL;

namespace TygaSoft.ThreadProcessor
{
    public class SmsProcessor
    {
        static int threadCount = 1;
        private static int transactionTimeout = int.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]);
        private static int queueTimeout = int.Parse(ConfigurationManager.AppSettings["QueueTimeout"]);
        private static int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

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
            try
            {
                TimeSpan tsTimeout = TimeSpan.FromSeconds(Convert.ToDouble(transactionTimeout * batchSize));
                SmsSend smsBll = new SmsSend();

                while (true)
                {
                    TimeSpan datetimeStarting = new TimeSpan(DateTime.Now.Ticks);
                    double elapsedTime = 0;
                    int processedItems = 0;

                    List<SmsSendInfo> smsList = new List<Model.SmsSendInfo>();
                    SmsTemplate smstBll = new SmsTemplate();
                    Order oBll = new Order();

                    for (int j = 0; j < batchSize; j++)
                    {
                        try
                        {
                            if ((elapsedTime + queueTimeout + transactionTimeout) < tsTimeout.TotalSeconds)
                            {
                                smsList.Add(smsBll.ReceiveFromQueue(queueTimeout));
                            }
                            else
                            {
                                j = batchSize;   // exit loop
                            }

                            //update elapsed time
                            elapsedTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds - datetimeStarting.TotalSeconds;
                        }
                        catch (TimeoutException)
                        {
                            //exit loop because no more messages are waiting
                            j = batchSize;
                        }
                    }

                    if (smsList.Count > 0)
                    {
                        foreach (var smsModel in smsList)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(smsModel.MobilePhone))
                                {
                                    string sendContent = "";

                                    Guid templateId = Guid.Empty;
                                    if (smsModel.SmsTemplateId != null) Guid.TryParse(smsModel.SmsTemplateId.ToString(), out templateId);
                                    if (!templateId.Equals(Guid.Empty))
                                    {
                                        #region 使用模板发送

                                        sendContent = smstBll.GetTemplateContent(templateId,"");

                                        if (sendContent == "")
                                        {
                                            WriteLog log = new WriteLog();
                                            log.Write("手机号：“" + smsModel.MobilePhone + "”，使用了模板（ID：" + templateId + "），但是模板不存在或已被删除，无法发送短信");
                                            continue;
                                        }

                                        #endregion
                                    }
                                    else if (!string.IsNullOrEmpty(smsModel.SmsContent))
                                    {
                                        sendContent = smsModel.SmsContent;
                                    }

                                    if (string.IsNullOrEmpty(sendContent))
                                    {
                                        WriteLog log = new WriteLog();
                                        log.Write("手机号：“" + smsModel.MobilePhone + "”，短信内容为空，无法发送短信");
                                        continue;
                                    }

                                    //发送短信并写入数据库
                                    smsModel.SmsContent = sendContent;
                                    SmsSendAndInsert(smsModel, null);
                                }

                                else if (!string.IsNullOrEmpty(smsModel.OrderCode))
                                {
                                    DataTable dt = null;
                                    string sendMobile = "";

                                    smsBll.GetSmsSendMobile(smsModel.OrderCode, smsModel.TranNode, out sendMobile, out dt);

                                    if (string.IsNullOrEmpty(sendMobile))
                                    {
                                        WriteLog log = new WriteLog();
                                        log.Write("订单号：" + smsModel.OrderCode + "，运输环节：" + smsModel.TranNodeText + ",异常：发货方或收货方手机号为空值或无效，无法发送短信");
                                        continue;
                                    }

                                    string[] mobiles = sendMobile.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    List<string> contentList = GetSmsContent(smsModel, dt);
                                    if (contentList.Count != mobiles.Length)
                                    {
                                        WriteLog log = new WriteLog();
                                        log.Write("订单号：" + smsModel.OrderCode + "，运输环节：" + smsModel.TranNodeText + ",异常：发货方或收货方短信模板找不到或已被删除，无法发送短信");
                                        continue;
                                    }

                                    for (var i = 0; i < mobiles.Length; i++)
                                    {
                                        smsModel.MobilePhone = mobiles[i];
                                        smsModel.SmsContent = contentList[i];

                                        SmsSendAndInsert(smsModel, dt);
                                    }
                                }
                                else if (!string.IsNullOrEmpty(smsModel.CarScanCode))
                                {
                                    string[] orders = oBll.GetOrderByCarcode(smsModel.CarScanCode);
                                    if (orders.Length == 0)
                                    {
                                        WriteLog log = new WriteLog();
                                        log.Write("派车单号：" + smsModel.CarScanCode + "，没有任何订单号，无法发送短信");
                                        continue;
                                    }
                                    foreach (string orderCode in orders)
                                    {
                                        DataTable dt = null;
                                        string sendMobile = "";

                                        smsBll.GetSmsSendMobile(orderCode, smsModel.TranNode == null ? "" : smsModel.TranNode, out sendMobile, out dt);

                                        if (string.IsNullOrEmpty(sendMobile))
                                        {
                                            WriteLog log = new WriteLog();
                                            log.Write("派车单号：“" + smsModel.CarScanCode + "”，订单号：" + orderCode + "，异常：发货方或收货方手机号为空值或无效，无法发送短信");
                                            continue;
                                        }

                                        string[] mobiles = sendMobile.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        List<string> contentList = GetSmsContent(smsModel, dt);
                                        if (contentList.Count != mobiles.Length)
                                        {
                                            WriteLog log = new WriteLog();
                                            log.Write("订单号：" + smsModel.OrderCode + "，运输环节：" + smsModel.TranNodeText + ",异常：发货方或收货方短信模板找不到或已被删除，无法发送短信");
                                            continue;
                                        }

                                        for (var i = 0; i < mobiles.Length; i++)
                                        {
                                            smsModel.MobilePhone = mobiles[i];
                                            smsModel.SmsContent = contentList[i];

                                            SmsSendAndInsert(smsModel, dt);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLog log = new WriteLog();
                                string errorAppend = "";
                                if (!string.IsNullOrEmpty(smsModel.MobilePhone)) errorAppend += "手机号：" + smsModel.MobilePhone + "";
                                if (!string.IsNullOrEmpty(smsModel.OrderCode)) errorAppend += "订单号：" + smsModel.OrderCode + "";
                                if (!string.IsNullOrEmpty(smsModel.CarScanCode)) errorAppend += "派车单号：" + smsModel.CarScanCode + "";
                                log.Write("异常：" + ex.Message + "，" + errorAppend + "");
                            }

                            processedItems++;
                        }
                    }

                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                WriteLog log = new WriteLog();
                log.Write(ex.Message);
            }
        }

        private static List<string> GetSmsContent(SmsSendInfo smsModel, DataTable dt)
        {
            List<string> list = new List<string>();
            string content = "";

            try
            {
                Guid templateId = Guid.Empty;
                if (smsModel.SmsTemplateId != null)
                {
                    Guid.TryParse(smsModel.SmsTemplateId.ToString(), out templateId);
                }
                if (!templateId.Equals(Guid.Empty))
                {
                    SmsTemplate smstBll = new SmsTemplate();
                    content = smstBll.GetTemplateContent(templateId, dt);
                    list.Add(content);
                }
                else if (!string.IsNullOrEmpty(smsModel.TemplateType))
                {
                    if (smsModel.TemplateType == "auto")
                    {
                        DataRow dr = dt.Rows[0];
                        DataColumnCollection dcc = dt.Columns;

                        string[] paramsCodeArr = smsModel.ParamsCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] tParmsValues = new string[paramsCodeArr.Length];
                        for (int i = 0; i < paramsCodeArr.Length; i++)
                        {
                            if (dcc.Contains(paramsCodeArr[i]))
                            {
                                if (dr[paramsCodeArr[i]] != DBNull.Value)
                                {
                                    tParmsValues[i] = dr[paramsCodeArr[i]].ToString();
                                }
                                else
                                {
                                    tParmsValues[i] = "";
                                }
                            }
                            else
                            {
                                tParmsValues[i] = "";
                            }
                        }

                        content = string.Format(smsModel.SmsContent, tParmsValues);
                        list.Add(content);
                    }
                    else if (smsModel.TemplateType == "custom")
                    {
                        content = string.Format(smsModel.SmsContent, smsModel.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        list.Add(content);
                    }
                }
                else if (!string.IsNullOrEmpty(smsModel.SmsContent))
                {
                    content = smsModel.SmsContent;
                    list.Add(content);
                }
                else
                {
                    SmsTemplate smstBll = new SmsTemplate();
                    return smstBll.GetTemplateContentByTranNode(dt, smsModel.TranNode);
                }
            }
            catch (Exception ex)
            {
                string error = "";
                error += "手机号：" + smsModel.MobilePhone == null ? "无" : smsModel.MobilePhone + "，";
                error += "订单号：" + smsModel.OrderCode == null ? "无" : smsModel.OrderCode + "，";
                error += "运输节点：" + smsModel.TranNodeText == null ? "无" : smsModel.TranNodeText + "，";
                error += "派车单号：" + smsModel.CarScanCode == null ? "无" : smsModel.CarScanCode + "，";
                error += "模板ID：" + smsModel.SmsTemplateId == null ? "无" : smsModel.SmsTemplateId.ToString() + "，";
                error += "参数代码：" + smsModel.ParamsCode == null ? "无" : smsModel.ParamsCode + "，";
                error += "参数名称：" + smsModel.ParamsName == null ? "无" : smsModel.ParamsName + "，";
                error += "参数值：" + smsModel.ParamsValue == null ? "无" : smsModel.ParamsValue + "，";
                error += "短信内容：" + smsModel.SmsContent == null ? "无" : smsModel.SmsContent;
                WriteLog log = new WriteLog();
                log.Write("执行获取短信内容时异常（"+ex.Message+"），详情：" + error + "");
            }

            return list;
        }

        /// <summary>
        /// 发送短信，并将已发送的信息写入数据库
        /// </summary>
        /// <param name="smsModel"></param>
        /// <param name="dt"></param>
        private static void SmsSendAndInsert(SmsSendInfo smsModel, DataTable dt)
        {
            try
            {
                SmsSend smssBll = new SmsSend();

                DataRow dr = null;
                if(dt != null && dt.Rows.Count > 0)
                {
                    dr = dt.Rows[0];
                }

                bool isSuccess = SmsHelper.SmsSendByPost(smsModel.MobilePhone, smsModel.SmsContent);

                DateTime currTime = DateTime.Now;
                string[] mobiles = smsModel.MobilePhone.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
                foreach (string mobile in mobiles)
                {
                    SmsSendInfo model = new SmsSendInfo();
                    model.UserId = smsModel.UserId;
                    model.OrderCode = smsModel.OrderCode == null ? "" : smsModel.OrderCode;
                    model.CarScanCode = smsModel.CarScanCode == null ? "" : smsModel.CarScanCode;
                    model.TranNode = smsModel.TranNodeText == null ? "" : smsModel.TranNodeText;
                    model.Receiver = smsModel.Receiver == null ? "" : smsModel.Receiver;
                    model.MobilePhone = smsModel.MobilePhone = mobile;
                    model.SmsContent = smsModel.SmsContent;
                    model.SendDate = currTime;
                    model.LastUpdatedDate = currTime;
                    model.SendStatus = (short)2;
                    if (isSuccess) model.SendStatus = (short)1;
                    model.Customer = "";
                    if (dr != null && dr["Customer"] != DBNull.Value)
                    {
                        model.Customer = dr["Customer"].ToString().Trim();
                    }

                    smssBll.Insert(model);
                }
            }
            catch (Exception ex)
            {
                string error = "";
                error += "手机号：" + smsModel.MobilePhone == null ? "无" : smsModel.MobilePhone + "，";
                error += "订单号：" + smsModel.OrderCode == null ? "无" : smsModel.OrderCode + "，";
                error += "运输节点：" + smsModel.TranNodeText == null ? "无" : smsModel.TranNodeText + "，";
                error += "派车单号：" + smsModel.CarScanCode == null ? "无" : smsModel.CarScanCode + "，";
                error += "短信内容：" + smsModel.SmsContent == null ? "无" : smsModel.SmsContent + "，";
                WriteLog log = new WriteLog();
                log.Write("执行添加短信发送数据到数据库异常（" + ex.Message + "），详情：" + error + "");
            }
        }
    }
}
