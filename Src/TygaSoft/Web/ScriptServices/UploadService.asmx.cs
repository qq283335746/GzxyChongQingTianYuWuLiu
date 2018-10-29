using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
//using LitJson;
//using TygaSoft.WebHelper;
//using NPOI.HSSF.UserModel;
//using NPOI.XSSF.UserModel;
//using NPOI.SS.UserModel;

namespace TygaSoft.Web.ScriptServices
{
    /// <summary>
    /// UploadService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tygaweb.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class UploadService : System.Web.Services.WebService
    {
        //HttpContext context = HttpContext.Current;
        //string filesRoot = ConfigHelper.GetValueByKey("FilesRoot").TrimEnd('/');

        #region Kindeditor

        //[WebMethod]
        //public void KindeditorFilesUpload()
        //{
        //    HttpPostedFile file = context.Request.Files["imgFile"];
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        UploadResult(false, "请选择文件!", "");
        //    }
        //    string dir = context.Request.QueryString["dir"];
        //    if (string.IsNullOrWhiteSpace(dir))
        //    {
        //        dir = "image";
        //    }

        //    try
        //    {
        //        if (!UploadFilesHelper.IsFileValidated(file.InputStream, file.ContentLength))
        //        {
        //            UploadResult(false, "该文件已被禁止上传", "");
        //        }
        //        string fileExt = Path.GetExtension(file.FileName).ToLower();

        //        string virtualPath = string.Empty;
        //        string fullPath = GetFullPath("Kindeditor/" + dir, fileExt, out virtualPath);

        //        file.SaveAs(fullPath);

        //        UploadResult(true, "", virtualPath.TrimStart('~'));
        //    }
        //    catch (Exception ex)
        //    {
        //        UploadResult(false, "上传异常：" + ex.Message, "");
        //    }
        //}

        //[WebMethod]
        //public void KindeditorFilesManager()
        //{
        //    string dirName = context.Request.QueryString["dir"];
        //    if (string.IsNullOrWhiteSpace(dirName))
        //    {
        //        dirName = "image";
        //    }
        //    if (Array.IndexOf("image,flash,media,file".Split(','), dirName) == -1)
        //    {
        //        UploadResult("无效的目录名。");
        //    }
        //    string virtualPath = filesRoot.TrimStart('~').TrimEnd('/') + "/Kindeditor/" + dirName;
        //    //图片扩展名
        //    String fileTypes = "gif,jpg,jpeg,png,bmp";
        //    string fullPath = Server.MapPath("~/" + virtualPath);

        //    string currentPath = "";
        //    string currentUrl = "";
        //    string currentDirPath = "";
        //    string moveupDirPath = "";

        //    //根据path参数，设置各路径和URL
        //    string path = context.Request.QueryString["path"];
        //    path = string.IsNullOrWhiteSpace(path) ? "" : path;
        //    if (path == "")
        //    {
        //        currentPath = fullPath;
        //        currentUrl = virtualPath;
        //        currentDirPath = "";
        //        moveupDirPath = "";
        //    }
        //    else
        //    {
        //        currentPath = fullPath + "\\" + path.TrimStart('\\');
        //        currentUrl = virtualPath + "/" + path;
        //        currentDirPath = path;
        //        moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
        //    }

        //    //排序形式，name or size or type
        //    string order = context.Request.QueryString["order"];
        //    order = string.IsNullOrEmpty(order) ? "" : order.ToLower();

        //    //不允许使用..移动到上一级目录
        //    if (Regex.IsMatch(path, @"\.\."))
        //    {
        //        UploadResult("禁止操作。");
        //    }
        //    //最后一个字符不是/
        //    if (path != "" && !path.EndsWith("/"))
        //    {
        //        UploadResult("参数无效。");
        //    }
        //    //目录不存在或不是目录
        //    if (!Directory.Exists(currentPath))
        //    {
        //        UploadResult("目录不存在。");
        //    }

        //    //遍历目录取得文件信息
        //    string[] dirList = Directory.GetDirectories(currentPath);
        //    string[] fileList = Directory.GetFiles(currentPath);

        //    switch (order)
        //    {
        //        case "size":
        //            Array.Sort(dirList, new NameSorter());
        //            Array.Sort(fileList, new SizeSorter());
        //            break;
        //        case "type":
        //            Array.Sort(dirList, new NameSorter());
        //            Array.Sort(fileList, new TypeSorter());
        //            break;
        //        case "name":
        //        default:
        //            Array.Sort(dirList, new NameSorter());
        //            Array.Sort(fileList, new NameSorter());
        //            break;
        //    }

        //    Hashtable result = new Hashtable();
        //    result["moveup_dir_path"] = moveupDirPath;
        //    result["current_dir_path"] = currentDirPath;
        //    result["current_url"] = currentUrl;
        //    result["total_count"] = dirList.Length + fileList.Length;
        //    List<Hashtable> dirFileList = new List<Hashtable>();
        //    result["file_list"] = dirFileList;
        //    for (int i = 0; i < dirList.Length; i++)
        //    {
        //        DirectoryInfo dir = new DirectoryInfo(dirList[i]);
        //        Hashtable hash = new Hashtable();
        //        hash["is_dir"] = true;
        //        hash["has_file"] = (dir.GetFileSystemInfos().Length > 0);
        //        hash["filesize"] = 0;
        //        hash["is_photo"] = false;
        //        hash["filetype"] = "";
        //        hash["filename"] = dir.Name;
        //        hash["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        //        dirFileList.Add(hash);
        //    }
        //    for (int i = 0; i < fileList.Length; i++)
        //    {
        //        FileInfo file = new FileInfo(fileList[i]);
        //        Hashtable hash = new Hashtable();
        //        hash["is_dir"] = false;
        //        hash["has_file"] = false;
        //        hash["filesize"] = file.Length;
        //        hash["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0);
        //        hash["filetype"] = file.Extension.Substring(1);
        //        hash["filename"] = file.Name;
        //        hash["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        //        dirFileList.Add(hash);
        //    }

        //    UploadResult(JsonMapper.ToJson(result));
        //}

        //private void UploadResult(string message)
        //{
        //    context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        //    context.Response.Write(message);
        //    context.ApplicationInstance.CompleteRequest();
        //}

        //private void UploadResult(bool isSuccess, string message, string url)
        //{
        //    int isSuccessValue = isSuccess ? 0 : 1;
        //    System.Collections.Hashtable hash = new System.Collections.Hashtable();
        //    hash["error"] = isSuccessValue;
        //    if (isSuccess) hash["url"] = url;
        //    else hash["message"] = message;

        //    context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        //    context.Response.Write(JsonMapper.ToJson(hash));
        //    context.ApplicationInstance.CompleteRequest();

        //    //return JsonMapper.ToJson(hash);
        //}

        //private string GetFullPath(string dir, string fileExt, out string virtualPath)
        //{
        //    string fileName = CustomsHelper.GetFormatDateTime();
        //    string fullPath = filesRoot;
        //    if (!string.IsNullOrWhiteSpace(dir))
        //    {
        //        fullPath += "/" + dir.TrimEnd('/');
        //    }
        //    fullPath += "/" + fileName.Substring(0, 6);

        //    virtualPath = fullPath + "/" + fileName + fileExt;

        //    fullPath = Server.MapPath(fullPath);

        //    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

        //    fullPath = fullPath + "\\" + fileName + fileExt;

        //    return fullPath;
        //}

        //class NameSorter : IComparer
        //{
        //    public int Compare(object x, object y)
        //    {
        //        if (x == null && y == null)
        //        {
        //            return 0;
        //        }
        //        if (x == null)
        //        {
        //            return -1;
        //        }
        //        if (y == null)
        //        {
        //            return 1;
        //        }
        //        FileInfo xInfo = new FileInfo(x.ToString());
        //        FileInfo yInfo = new FileInfo(y.ToString());

        //        return xInfo.FullName.CompareTo(yInfo.FullName);
        //    }
        //}

        //class SizeSorter : IComparer
        //{
        //    public int Compare(object x, object y)
        //    {
        //        if (x == null && y == null)
        //        {
        //            return 0;
        //        }
        //        if (x == null)
        //        {
        //            return -1;
        //        }
        //        if (y == null)
        //        {
        //            return 1;
        //        }
        //        FileInfo xInfo = new FileInfo(x.ToString());
        //        FileInfo yInfo = new FileInfo(y.ToString());

        //        return xInfo.Length.CompareTo(yInfo.Length);
        //    }
        //}

        //class TypeSorter : IComparer
        //{
        //    public int Compare(object x, object y)
        //    {
        //        if (x == null && y == null)
        //        {
        //            return 0;
        //        }
        //        if (x == null)
        //        {
        //            return -1;
        //        }
        //        if (y == null)
        //        {
        //            return 1;
        //        }
        //        FileInfo xInfo = new FileInfo(x.ToString());
        //        FileInfo yInfo = new FileInfo(y.ToString());

        //        return xInfo.Extension.CompareTo(yInfo.Extension);
        //    }
        //}

        #endregion

        #region uploadify 上传

        //[WebMethod]
        //public string SmsPhoneUpload()
        //{
        //    //return "hello word";
        //    HttpPostedFile file = context.Request.Files["Filedata"];
        //    if (file == null && file.ContentLength == 0)
        //    {
        //        return "请选择文件并上传";
        //    }

        //    List<string> list = new List<string>();

        //    //IWorkbook wb = null;
        //    //ISheet sheet = null;
        //    try
        //    {
        //        //string fex = Path.GetExtension(file.FileName).ToLower();
        //        //wb = fex == "xls" ? new HSSFWorkbook(file.InputStream) as IWorkbook : new XSSFWorkbook(file.InputStream) as IWorkbook;
        //        //sheet = wb.GetSheetAt(0);
        //        //IEnumerator rows = sheet.GetRowEnumerator();
        //        //int index = -1;
        //        //while (rows.MoveNext())
        //        //{
        //        //    index++;
        //        //    //过滤第一行标题行
        //        //    if (index == 0) continue;
        //        //    IRow row = rows.Current as IRow;
        //        //    List<ICell> cells = row.Cells;
        //        //    foreach (ICell cell in cells)
        //        //    {
        //        //        string mobile = cell.NumericCellValue.ToString();
        //        //        if (Regex.IsMatch(mobile, @"(\d+){11,15}"))
        //        //        {
        //        //            if (!list.Contains(cell.NumericCellValue.ToString()))
        //        //            {

        //        //                list.Add(cell.NumericCellValue.ToString());
        //        //            }
        //        //        }
        //        //    }
        //        //}

        //        //UploadFilesHelper ufh = new UploadFilesHelper();
        //        //string fileUrl = ufh.UploadToTemp(files);
        //        //using (FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        //{
        //        //    hssfWorkbook = new HSSFWorkbook(files.InputStream);
        //        //}
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //    return string.Join(",", list);
        //}

        #endregion
    }
}
