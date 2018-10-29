using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;

namespace TygaSoft.ThreadProcessor
{
    public class SmsHelper
    {
        private static readonly string smsUser = ConfigurationManager.AppSettings["SmsUser"];
        private static readonly string smsPassword = ConfigurationManager.AppSettings["SmsPassword"];
        private static readonly string smsServer = ConfigurationManager.AppSettings["SmsServer"];

        /// <summary>
        /// 使用POST方式发送短信
        /// </summary>
        /// <param name="phone">多个手机号使用“,”分割</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SmsSendByPost(string phone, string content)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(content)) return false;

            int statusCode = -1;
            string result = "";

            string postData = "";
            postData += "id=" + smsUser;
            postData += "&pwd=" + smsPassword;
            postData += "&to=" + phone;
            postData += "&content=" + content;
            postData += "&time=";

            DoPost(postData, out statusCode, out result);

            string[] resultArr = result.Split('/');
            if (resultArr[0] == "000") return true;

            return false;
        }

        /// <summary>
        /// POST方式发送
        /// </summary>
        /// <param name="content"></param>
        /// <param name="statusCode"></param>
        /// <param name="result"></param>
        private static void DoPost(string content, out int statusCode, out string result)
        {
            statusCode = -1;
            result = string.Empty;

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                Encoding encoding = Encoding.GetEncoding("gb2312");
                byte[] data = encoding.GetBytes(content);

                request = (HttpWebRequest)WebRequest.Create(smsServer);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                var streamReader = new StreamReader(responseStream);
                result = streamReader.ReadToEnd();
                statusCode = (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                WriteLog log = new WriteLog();
                log.Write(ex.Message);
            }
        }

        #region 测试成功例子 暂不使用

        ///// <summary>
        ///// 使用GET方式请求
        ///// </summary>
        //private static void DoGet()
        //{
        //    string uid = "13828439516";
        //    string pwd = "13828439516";
        //    string phone = "18308909020";
        //    string content = "订单号、收、发货单位、联系人、手机、地址、业务日期（带时间与不带时间两种）、委托客户、总箱数、起运地、目的地、物流产品、服务网点、PDA扫描类型、PDA扫描时间、PDA登录人员网点、派车单号、车主";
        //    content = "你好 天宇物流订单查询";
        //    Uri uri = new Uri("http://service.winic.org/sys_port/gateway/?id=" + uid + "&pwd=" + pwd + "&to=" + phone + "&content=" + content + "&time=");

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.Method = "GET";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    string result = "";
        //    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
        //    {
        //        result = sr.ReadToEnd();
        //        result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        //    }
        //    int status = (int)response.StatusCode;

        //    //返回数据
        //    //000/Send:3/Consumption:.3/Tmoney:28.7/sid:0910221219157990
        //}

        ///// <summary>
        ///// 使用POST方式请求
        ///// </summary>
        //private static void DoPost()
        //{
        //    string uid = "13828439516";
        //    string pwd = "13828439516";
        //    string phone = "18308909020";
        //    string content = "订单号、收、发货单位、联系人、手机、地址、业务日期（带时间与不带时间两种）、委托客户、总箱数、起运地、目的地、物流产品、服务网点、PDA扫描类型、PDA扫描时间、PDA登录人员网点、派车单号、车主";
        //    //content = "你好 天宇物流订单查询";

        //    string postData = "";
        //    postData += "id=" + uid;
        //    postData += "&pwd=" + pwd;
        //    postData += "&to=" + phone;
        //    postData += "&content=" + content;
        //    postData += "&time=";

        //    Encoding encoding = Encoding.GetEncoding("gb2312");
        //    byte[] data = encoding.GetBytes(postData);

        //    string url = "http://service.winic.org/sys_port/gateway/";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = data.Length;
        //    Stream reqStream = request.GetRequestStream();
        //    reqStream.Write(data, 0, data.Length);
        //    reqStream.Close();

        //    //request.ContentLength = postData.Length;
        //    //var sw = new StreamWriter(request.GetRequestStream());
        //    //sw.Write(postData);
        //    //sw.Close();

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Stream responseStream = response.GetResponseStream();
        //    var streamReader = new StreamReader(responseStream);
        //    string result = streamReader.ReadToEnd();

        //    //string result = "";
        //    //using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
        //    //{
        //    //    result = sr.ReadToEnd();
        //    //    result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        //    //}
        //    int status = (int)response.StatusCode;
        //}

        #endregion
    }
}
