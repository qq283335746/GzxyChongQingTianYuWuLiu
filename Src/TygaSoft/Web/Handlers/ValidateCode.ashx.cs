﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace TygaSoft.Web.Handlers
{
    /// <summary>
    /// ValidateCode 的摘要说明
    /// </summary>
    public class ValidateCode : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);//特别注意，如不加，单击验证图片＇看不清换一张＇，无效果．           
            this.CreateCheckCodeImage(GenerateCheckCode(context), context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region 验证码开始

        private string GenerateCheckCode(HttpContext context)
        {
            int number;
            char code;
            string checkCode = String.Empty;
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                number = random.Next();
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else if (number % 3 == 0)
                    code = (char)('A' + (char)(number % 26));
                else
                    code = (char)('a' + (char)(number % 26));
                checkCode += code.ToString();
            }

            string cookieName = GetCookieName(context);
            if (cookieName != "")
            {
                context.Response.Cookies["ValidateCode"][cookieName] = checkCode;
            }

            return checkCode;
        }

        private void CreateCheckCodeImage(string checkCode, HttpContext context)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {                 //生成随机生成器               
                Random random = new Random();

                //清空图片背景色               
                g.Clear(Color.White);

                //画图片的背景噪音线                
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true); g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点                
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线                
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                context.Response.ClearContent();
                context.Response.ContentType = "image/Gif";
                context.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }

        }

        /// <summary>
        /// 获取验证码存储的Cookie名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetCookieName(HttpContext context)
        {
            string cookieName = context.Request.QueryString["vcType"];
            if (!string.IsNullOrWhiteSpace(cookieName))
            {
                switch (cookieName)
                {
                    case "dlgLogin":
                        return "DlgLoginVc";
                    case "login":
                        return "LoginVc";
                    case "register":
                        return "RegisterVc";
                    case "pswReset":
                        return "PswResetVc";
                    case "addUser":
                        return "AddUserVc";
                    default:
                        return "";
                }
            }

            return "ChechCode";
        }

        #endregion 验证码结束
    }
}