using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using TygaSoft.WebHelper;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace TygaSoft.Web.Handlers
{
    /// <summary>
    /// HandlerUpload 的摘要说明
    /// </summary>
    public class HandlerUpload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            SmsPhoneUpload(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void SmsPhoneUpload(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            HttpPostedFile file = context.Request.Files["Filedata"];
            if (file == null || file.ContentLength == 0)
            {
                context.Response.Write("异常：请选择文件并上传");
            }

            List<string> list = new List<string>();

            IWorkbook wb = null;
            ISheet sheet = null;
            try
            {
                string fex = Path.GetExtension(file.FileName).ToLower();
                wb = fex == "xls" ? new HSSFWorkbook(file.InputStream) as IWorkbook : new XSSFWorkbook(file.InputStream) as IWorkbook;
                sheet = wb.GetSheetAt(0);
                IEnumerator rows = sheet.GetRowEnumerator();
                int index = -1;
                while (rows.MoveNext())
                {
                    index++;
                    //过滤第一行标题行
                    if (index == 0) continue;
                    IRow row = rows.Current as IRow;
                    List<ICell> cells = row.Cells;
                    foreach (ICell cell in cells)
                    {
                        string mobile = cell.NumericCellValue.ToString();
                        if (Regex.IsMatch(mobile, @"(\d+){11,15}"))
                        {
                            if (!list.Contains(cell.NumericCellValue.ToString()))
                            {

                                list.Add(cell.NumericCellValue.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("异常： " + ex.Message + "");
            }

            context.Response.Write(string.Join(",", list));
        }
    }
}