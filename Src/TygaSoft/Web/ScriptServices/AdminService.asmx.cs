using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Text;
using System.Transactions;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using TygaSoft.CustomProvider;
using TygaSoft.Model;
using TygaSoft.BLL;
using TygaSoft.DBUtility;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.ScriptServices
{
    /// <summary>
    /// AdminService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tygaweb.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class AdminService : System.Web.Services.WebService
    {
        HttpContext context = HttpContext.Current;

        #region 系统枚举配置

        [WebMethod]
        public string GetJsonForSysEnum()
        {
            SysEnum bll = new SysEnum();
            return bll.GetTreeJson();
        }

        [WebMethod]
        public string SaveSysEnum(SysEnumInfo sysEnumModel)
        {
            if (string.IsNullOrWhiteSpace(sysEnumModel.EnumName))
            {
                return MessageContent.Submit_Params_InvalidError;
            }
            if (string.IsNullOrWhiteSpace(sysEnumModel.EnumCode))
            {
                return MessageContent.Submit_Params_InvalidError;
            }
            if (string.IsNullOrWhiteSpace(sysEnumModel.EnumValue))
            {
                return MessageContent.Submit_Params_InvalidError;
            }

            Guid gId = Guid.Empty;
            Guid.TryParse(sysEnumModel.Id.ToString(), out gId);
            sysEnumModel.Id = gId;

            Guid parentId = Guid.Empty;
            Guid.TryParse(sysEnumModel.ParentId.ToString(), out parentId);
            sysEnumModel.ParentId = parentId;

            SysEnum bll = new SysEnum();
            int effect = -1;

            if (!gId.Equals(Guid.Empty))
            {
                effect = bll.Update(sysEnumModel);
            }
            else
            {
                effect = bll.Insert(sysEnumModel);
            }
            
            if (effect == 110)
            {
                return MessageContent.Submit_Exist;
            }
            if (effect > 0)
            {
                return "1";
            }
            else return MessageContent.Submit_Error;
        }

        [WebMethod]
        public string DelSysEnum(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return MessageContent.Submit_InvalidRow;
            }
            Guid gId = Guid.Empty;
            Guid.TryParse(id, out gId);
            if (gId.Equals(Guid.Empty))
            {
                return MessageContent.GetString(MessageContent.Submit_Params_GetInvalidRegex,"对应标识值");
            }
            SysEnum bll = new SysEnum();
            bll.Delete(gId);
            return "1";
        }

        #endregion

        #region 分类管理

        [WebMethod]
        public string GetJsonForCategory()
        {
            Category bll = new Category();
            return bll.GetTreeJson();
        }

        [WebMethod]
        public string SaveCategory(CategoryInfo categoryModel)
        {
            if (string.IsNullOrWhiteSpace(categoryModel.CategoryName))
            {
                return "带有“*”的为必填项，请检查";
            }
            categoryModel.CategoryName = categoryModel.CategoryName.Trim();

            if (string.IsNullOrWhiteSpace(categoryModel.CategoryCode))
            {
                return "带有“*”的为必填项，请检查";
            }

            categoryModel.CategoryCode = categoryModel.CategoryCode.Trim();
            int sort = 0;
            int.TryParse(categoryModel.Sort.ToString(), out sort);
            categoryModel.Sort = sort;
            categoryModel.Remark = categoryModel.Remark.Trim();

            Guid gId = Guid.Empty;
            Guid.TryParse(categoryModel.Id.ToString(), out gId);
            categoryModel.Id = gId;

            Guid parentId = Guid.Empty;
            Guid.TryParse(categoryModel.ParentId.ToString(), out parentId);
            categoryModel.ParentId = parentId;

            categoryModel.LastUpdatedDate = DateTime.Now;

            Category bll = new Category();
            int effect = -1;

            if (!gId.Equals(Guid.Empty))
            {
                effect = bll.Update(categoryModel);
            }
            else
            {
                effect = bll.Insert(categoryModel);
            }

            if (effect == 110)
            {
                return "操作失败，原因：已存在相同记录";
            }
            if (effect > 0)
            {
                return "1";
            }
            else return "操作失败，原因：系统异常";
        }

        [WebMethod]
        public string DelCategory(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return "操作失败，原因：请选择要删除的数据";
            }
            Guid gId = Guid.Empty;
            Guid.TryParse(id, out gId);
            if (gId.Equals(Guid.Empty))
            {
                return "操作失败，原因：获取对应标识值不正确，请检查";
            }
            Category bll = new Category();
            bll.Delete(gId);
            return "1";
        }

        #endregion

        #region 站点内容类型

        [WebMethod]
        public string GetJsonForContentType()
        {
            ContentType bll = new ContentType();
            return bll.GetTreeJson();
        }

        [WebMethod]
        public string SaveContentType(ContentTypeInfo contentTypeModel)
        {
            if (string.IsNullOrWhiteSpace(contentTypeModel.TypeName))
            {
                return "带有“*”的为必填项，请检查";
            }
            contentTypeModel.TypeName = contentTypeModel.TypeName.Trim();

            if (string.IsNullOrWhiteSpace(contentTypeModel.TypeCode))
            {
                return "带有“*”的为必填项，请检查";
            }

            contentTypeModel.TypeCode = contentTypeModel.TypeCode.Trim();
            int sort = 0;
            int.TryParse(contentTypeModel.Sort.ToString(), out sort);
            contentTypeModel.Sort = sort;

            Guid gId = Guid.Empty;
            Guid.TryParse(contentTypeModel.Id.ToString(), out gId);
            contentTypeModel.Id = gId;

            Guid parentId = Guid.Empty;
            Guid.TryParse(contentTypeModel.ParentId.ToString(), out parentId);
            contentTypeModel.ParentId = parentId;

            contentTypeModel.LastUpdatedDate = DateTime.Now;

            ContentType bll = new ContentType();
            int effect = -1;

            if (!gId.Equals(Guid.Empty))
            {
                effect = bll.Update(contentTypeModel);
            }
            else
            {
                effect = bll.Insert(contentTypeModel);
            }

            if (effect == 110)
            {
                return "操作失败，原因：已存在相同记录";
            }
            if (effect > 0)
            {
                return "1";
            }
            else return "操作失败，原因：系统异常";
        }

        [WebMethod]
        public string DelContentType(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return "操作失败，原因：请选择要删除的数据";
            }
            Guid gId = Guid.Empty;
            Guid.TryParse(id, out gId);
            if (gId.Equals(Guid.Empty))
            {
                return "操作失败，原因：获取对应标识值不正确，请检查";
            }
            ContentType bll = new ContentType();
            bll.Delete(gId);
            return "1";
        }

        #endregion

        #region 菜单导航

        [WebMethod]
        public string GetTreeJsonForMenu()
        {
            string[] roles = Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name);
            SitemapHelper.Roles = roles.ToList();
            return SitemapHelper.GetTreeJsonForMenu();
        }

        #endregion 

        #region 用户角色

        [WebMethod]
        public string SaveRole(RoleInfo model)
        {
            model.RoleName = model.RoleName.Trim();
            if (string.IsNullOrEmpty(model.RoleName))
            {
                return MessageContent.Submit_Params_InvalidError;
            }

            if (Roles.RoleExists(model.RoleName))
            {
                return MessageContent.Submit_Exist;
            }

            Guid gId = Guid.Empty;
            if (model.RoleId != null)
            {
                Guid.TryParse(model.RoleId.ToString(), out gId);
            }

            try
            {

                Role bll = new Role();

                if (!gId.Equals(Guid.Empty))
                {
                    bll.Update(model);
                }
                else
                {
                    Roles.CreateRole(model.RoleName);
                }

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string DelRole(string itemAppend)
        {
            itemAppend = itemAppend.Trim();
            if (string.IsNullOrEmpty(itemAppend))
            {
                return MessageContent.Submit_InvalidRow;
            }
            try
            {
                string[] roleIds = itemAppend.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in roleIds)
                {
                    Roles.DeleteRole(item);
                }

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string SaveIsLockedOut(string userName)
        {
            try
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user == null)
                {
                    return "当前用户不存在，请检查";
                }
                if (user.IsLockedOut)
                {
                    if (user.UnlockUser())
                    {
                        return "0";
                    }
                    else
                    {
                        return "操作失败，请联系管理员";
                    }
                }

                return "只有“已锁定”的用户才能执行此操作";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string SaveIsApproved(string userName)
        {
            try
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user == null)
                {
                    return "当前用户不存在，请检查";
                }
                if (user.IsApproved)
                {
                    user.IsApproved = false;
                }
                else
                {
                    user.IsApproved = true;
                }

                Membership.UpdateUser(user);

                return user.IsApproved ? "1" : "0";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string SaveUserInRole(string userName, string roleName, bool isInRole)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return MessageContent.GetString(MessageContent.Request_InvalidArgument, "用户名");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return MessageContent.GetString(MessageContent.Request_InvalidArgument, "角色");
            }
            try
            {
                if (isInRole)
                {
                    if (!Roles.IsUserInRole(userName, roleName))
                    {
                        Roles.AddUserToRole(userName, roleName);
                    }
                }
                else
                {
                    if (Roles.IsUserInRole(userName, roleName))
                    {
                        Roles.RemoveUserFromRole(userName, roleName);
                    }
                }
                return "1";
            }
            catch (System.Configuration.Provider.ProviderException pex)
            {
                return pex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string GetUserInRole(string userName)
        {
            try
            {
                string[] roles = Roles.GetRolesForUser(userName);
                if (roles.Length == 0) return "";

                return string.Join(",", roles);
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string DelUser(string userName)
        {
            try
            {
                Membership.DeleteUser(userName);
                return "1";
            }
            catch (Exception ex)
            {
                return "" + MessageContent.AlertTitle_Ex_Error + "：" + ex.Message;
            }
        }

        [WebMethod]
        public string SaveUser(UserInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                return MessageContent.Submit_Params_InvalidError;
            }
            if (model.Password != model.CfmPsw)
            {
                return MessageContent.Request_InvalidCompareToPassword;
            }
            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                model.Email = model.UserName + "tygaweb.com";
            }

            try
            {
                model.RoleName = model.RoleName.Trim().Trim(',');
                string[] roles = null;
                if(!string.IsNullOrEmpty(model.RoleName))
                {
                    roles = model.RoleName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }

                MembershipCreateStatus status;
                MembershipUser user;

                using (TransactionScope scope = new TransactionScope())
                {
                    user = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, model.IsApproved, out status);
                    if (roles != null && roles.Length > 0)
                    {
                        Roles.AddUserToRoles(model.UserName, roles);
                    }

                    scope.Complete();
                }
                
                if (user == null)
                {
                    return EnumMembershipCreateStatus.GetStatusMessage(status);
                }

                return "1";
            }
            catch (MembershipCreateUserException ex)
            {
                return EnumMembershipCreateStatus.GetStatusMessage(ex.StatusCode);
            }
            catch (HttpException ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region 手机短信（sms）

        [WebMethod]
        public string SaveSmgTemplate(SmsTemplateInfo model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.SmsContent))
                {
                    return MessageContent.Submit_Params_InvalidError;
                }

                model.Title = model.Title.Trim();
                model.ParamsCode = model.ParamsCode.Trim();
                model.ParamsName = model.ParamsName.Trim();
                model.ParamsValue = model.ParamsValue.Trim();
                model.SmsContent = model.SmsContent.Trim();
                model.TemplateType = model.TemplateType.Trim();

                if (model.TemplateType != "")
                {
                    if (model.ParamsCode == "" || model.ParamsValue == "" || model.ParamsName == "")
                    {
                        return "模板类型不为空字符串时，则已选参数不能为空字符串";
                    }
                }

                try
                {
                    if (model.TemplateType == "auto")
                    {
                        string content = string.Format(model.SmsContent, model.ParamsCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (model.TemplateType == "custom")
                    {
                        string content = string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
                catch
                {
                    return "保存失败，原因：已选参数与短信内容匹配有错，请正确操作";
                }

                model.LastUpdatedDate = DateTime.Now;
                model.UserId = Common.GetUserId(context);

                SmsTemplate smstBll = new SmsTemplate();

                if (model.Id != null && !string.IsNullOrEmpty(model.Id.ToString()))
                {
                    Guid gId = Guid.Empty;
                    Guid.TryParse(model.Id.ToString(), out gId);
                    if (gId.Equals(Guid.Empty))
                    {
                        return MessageContent.GetString(MessageContent.Request_InvalidArgument, "当前数据行主键标识");
                    }

                    model.Id = gId;
                    smstBll.Update(model);
                }
                else
                {
                    smstBll.Insert(model);
                }

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string DelSmgTemplate(string itemAppend)
        {
            itemAppend = itemAppend.Trim();
            if (string.IsNullOrEmpty(itemAppend))
            {
                return MessageContent.Submit_InvalidRow;
            }
            try
            {
                SmsTemplate smstBll = new SmsTemplate();
                string[] Ids = itemAppend.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                smstBll.DeleteBatch(Ids.ToList<object>());

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string SetDefaultSmgTemplate(object Id)
        {
            try
            {
                if (Id == null)
                {
                    return MessageContent.Request_InvalidArgument;
                }

                Guid gId = Guid.Empty;
                if (!Guid.TryParse(Id.ToString(), out gId))
                {
                    return MessageContent.Request_InvalidArgument;
                }

                SmsTemplate smstBll = new SmsTemplate();

                using (TransactionScope scope = new TransactionScope())
                {
                    smstBll.SetDefault(gId);

                    scope.Complete();
                }

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string DelSmsSend(string itemAppend)
        {
            itemAppend = itemAppend.Trim();
            if (string.IsNullOrEmpty(itemAppend))
            {
                return MessageContent.Submit_InvalidRow;
            }
            try
            {
                SmsSend smssBll = new SmsSend();
                string[] Ids = itemAppend.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                smssBll.DeleteBatch(Ids.ToList<object>());

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string GetJsonBySmgTemplate(int pageIndex, int pageSize, string title)
        {
            string json = "";
            if(pageIndex < 1) pageIndex = 1;
            if(pageSize < 1) pageSize = 10;
            int totalRecords = 0;
            string sqlWhere = "";
            try
            {
                ParamsHelper parms = new ParamsHelper();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sqlWhere += "and Title like @Title ";
                    SqlParameter parm = new SqlParameter("@Title", SqlDbType.NVarChar, 50);
                    parm.Value = "%" + title + "%";

                    parms.Add(parm);
                }
                SmsTemplate bll = new SmsTemplate();
                var list = bll.GetList(pageIndex, pageSize, out totalRecords, sqlWhere,parms.ToArray());
                if (list != null && list.Count > 0)
                {
                    string itemAppend = "";
                    foreach (var model in list)
                    {
                        string isAutoText = "";
                        if (model.TemplateType == "auto") isAutoText = "自动";
                        else if (model.TemplateType == "custom") isAutoText = "自定义";
                        string isDefaultText = model.IsDefault ? "是" : "否";
                        itemAppend += "{\"Id\":\"" + model.Id + "\",\"Title\":\"" + model.Title + "\",\"IsAuto\":\"" + isAutoText + "\",\"IsDefault\":\"" + isDefaultText + "\"},";
                    }

                    itemAppend = itemAppend.Trim(',');
                    itemAppend = "{\"total\":" + totalRecords + ",\"rows\":[" + itemAppend + "]}";

                    json = itemAppend;
                }

                return json;
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string SmsSend(SmsSendInfo model)
        {
            if (string.IsNullOrEmpty(model.SmsContent.Trim()))
            {
                return "发送失败，短信内容不能为空";
            }
            try
            {
                model.TemplateType = model.TemplateType.Trim();
                model.SmsTemplateId = model.SmsTemplateId.ToString().Trim();
                model.SmsContent = model.SmsContent.Trim();
                model.NumberType = model.NumberType.Trim();
                model.NumberCode = model.NumberCode.Trim();
                model.ParamsCode = model.ParamsCode.Trim();
                model.ParamsName = model.ParamsName.Trim();
                model.ParamsValue = model.ParamsValue.Trim();
                model.TranNode = model.TranNode.Trim();

                #region 短信内容与参数对应检查

                try
                {
                    if (model.SmsTemplateId.ToString() != "")
                    {
                        Guid templateId = Guid.Empty;
                        Guid.TryParse(model.SmsTemplateId.ToString(), out templateId);
                        if (templateId.Equals(Guid.Empty))
                        {
                            return MessageContent.GetString(MessageContent.Request_Data_Invalid, "当前模板");
                        }
                        SmsTemplate smstBll = new SmsTemplate();
                        var smstModel = smstBll.GetModel(templateId);
                        if (smstModel == null)
                        {
                            return MessageContent.GetString(MessageContent.Request_Data_Invalid, "当前模板");
                        }
                        if (smstModel.TemplateType == "auto")
                        {
                            if (model.NumberType == "MobilePhone")
                            {
                                return "发送失败，原因：当号码类型为手机号码时，则不能选择自动类型的模板";
                            }
                            string content = string.Format(model.SmsContent, model.ParamsCode.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                        else if (model.TemplateType == "custom")
                        {
                            string content = string.Format(model.SmsContent, model.ParamsValue.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                    else
                    {
                        if (model.TemplateType == "auto")
                        {
                            if (model.NumberType == "MobilePhone")
                            {
                                return "发送失败，原因：当号码类型为手机号码时，则不能选择自动类型的模板";
                            }
                            string content = string.Format(model.SmsContent, model.ParamsCode.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                        else if (model.TemplateType == "custom")
                        {
                            string content = string.Format(model.SmsContent, model.ParamsValue.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }
                }
                catch
                {
                    return "发送失败，原因：已选参数与短信内容匹配有错，请正确操作";
                }

                #endregion

                SmsSendInfo ssiModel = new SmsSendInfo();
                ssiModel.TemplateType = model.TemplateType;
                ssiModel.TranNode = model.TranNode.Trim();
                ssiModel.TranNodeText = model.TranNodeText.Trim();
                ssiModel.ParamsCode = model.ParamsCode;
                ssiModel.ParamsName = model.ParamsName;
                ssiModel.ParamsValue = model.ParamsValue;
                ssiModel.SmsContent = model.SmsContent.Trim();
                ssiModel.SmsTemplateId = model.SmsTemplateId;

                switch (model.NumberType)
                {
                    case "MobilePhone":
                        ssiModel.MobilePhone = model.NumberCode;
                        if (!Regex.IsMatch(ssiModel.MobilePhone, @"^(\d+){11,15}$"))
                        {
                            return MessageContent.GetString(MessageContent.Request_InvalidValue, "手机号码");
                        }
                        break;
                    case "OrderCode":
                        ssiModel.OrderCode = model.NumberCode;
                        break;
                    case "CarScanCode":
                        ssiModel.CarScanCode = model.NumberCode;
                        break;
                    default:
                        break;
                }

                SmsSend ssBll = new SmsSend();
                ssBll.InsertByStrategy(ssiModel);

                return "1";
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string SmsSendByImport(SmsSendInfo model)
        {
            if (string.IsNullOrEmpty(model.SmsContent.Trim()))
            {
                return "发送失败，短信内容不能为空";
            }
            if (string.IsNullOrWhiteSpace(model.MobilePhone))
            {
                return "请导入手机号码";
            }

            List<string> mobileList = new List<string>();

            try
            {
                model.MobilePhone = model.MobilePhone.Trim();
                string[] mobilePhoneArr = model.MobilePhone.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in mobilePhoneArr)
                {
                    if (Regex.IsMatch(item, @"(\d+){11,15}"))
                    {
                        if(!mobileList.Contains(item))
                        {
                            mobileList.Add(item);
                        }
                    }
                }

                if (mobileList.Count == 0)
                {
                    return "无任何正确的手机号码，无法发送短信";
                }

                Guid templateId = Guid.Empty;
                if (model.SmsTemplateId != null)
                {
                    Guid.TryParse(model.SmsTemplateId.ToString(), out templateId);
                }

                try
                {
                    if (!templateId.Equals(Guid.Empty))
                    {
                        SmsTemplate smstBll = new SmsTemplate();
                        var smstModel = smstBll.GetModel(model.SmsTemplateId);
                        if (smstModel == null)
                        {
                            return "发送失败，原因：模板不存在或已被删除";
                        }
                        if (smstModel.TemplateType == "auto")
                        {
                            return "发送失败，原因：不能选择自动类型的模板";
                        }

                        model.SmsContent = smstModel.SmsContent;
                    }
                    else
                    {
                        model.SmsContent = string.Format(model.SmsContent, model.ParamsValue.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
                catch
                {
                    return "发送失败，原因：已选参数与短信内容匹配有错，请正确操作";
                }

                SmsSend ssBll = new SmsSend();

                if (mobileList.Count > 100)
                {
                    while (mobileList.Count > 0)
                    {
                        var q = mobileList.Take(100);

                        if (q.Count() > 0)
                        {
                            var currList = q.ToList<string>();

                            SmsSendInfo ssiModel = new SmsSendInfo();
                            ssiModel.MobilePhone = string.Join(",", currList);
                            ssiModel.MobilePhone = model.MobilePhone;
                            ssiModel.SmsContent = model.SmsContent.Trim();

                            ssBll.InsertByStrategy(ssiModel);

                            foreach (var item in currList)
                            {
                                mobileList.Remove(item);
                            }
                        }
                    }
                }
                else
                {
                    SmsSendInfo ssiModel = new SmsSendInfo();
                    ssiModel.MobilePhone = model.MobilePhone;
                    ssiModel.SmsContent = model.SmsContent.Trim();

                    ssBll.InsertByStrategy(ssiModel);
                }

                return "1";
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string PreviewTemplate(string orderCode,object templateId)
        {
            try
            {
                string content = "";

                if (templateId.ToString() == "")
                {
                    return MessageContent.GetString(MessageContent.Request_Data_Invalid, "当前模板");
                }

                Guid gId = Guid.Empty;
                Guid.TryParse(templateId.ToString(), out gId);
                if (gId.Equals(Guid.Empty)) return MessageContent.GetString(MessageContent.Request_InvalidArgument, "模板ID");

                SmsTemplate smstBll = new SmsTemplate();
                var model = smstBll.GetModel(gId);
                if (model == null) return MessageContent.GetString(MessageContent.Request_Data_Invalid, "当前模板");
                if (model.TemplateType == "auto")
                {
                    if (string.IsNullOrEmpty(orderCode.Trim()))
                    {
                        return "当前模板为自动类型，订单号为必填项";
                    }

                    return smstBll.GetTemplateContent(model.Id, orderCode);
                }
                else if (model.TemplateType == "custom")
                {
                    content = string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    content = model.SmsContent;
                }

                return content;
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string PreviewSmsSend(SmsSendInfo model)
        {
            try
            {
                model.NumberType = model.NumberType.Trim();
                model.SmsTemplateId = model.SmsTemplateId.ToString().Trim();
                model.NumberCode = model.NumberCode.Trim();
                model.ParamsCode = model.ParamsCode.Trim();

                if (model.SmsTemplateId.ToString() != "")
                {
                    #region 使用模板

                    Guid templateId = Guid.Empty;
                    Guid.TryParse(model.SmsTemplateId.ToString(), out templateId);
                    if (templateId.Equals(Guid.Empty)) return MessageContent.GetString(MessageContent.Request_InvalidArgument, "模板ID");

                    SmsTemplate smstBll = new SmsTemplate();
                    var smstModel = smstBll.GetModel(templateId);
                    if (smstModel == null) return MessageContent.GetString(MessageContent.Request_Data_Invalid, "当前模板");
                    if (smstModel.TemplateType == "auto")
                    {
                        if (model.NumberType != "OrderCode")
                        {
                            return "当选择的模板类型为自动类型时，号码类型必须是订单号才能预览";
                        }

                        return smstBll.GetTemplateContent(model.SmsTemplateId, model.NumberCode);
                    }
                    else if (model.TemplateType == "custom")
                    {
                        return string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        return model.SmsContent;
                    }

                    #endregion
                }
                else if (model.TemplateType == "auto")
                {
                    if (model.NumberType != "OrderCode" || model.ParamsCode == "")
                    {
                        return "当选择的模板类型为自动类型时，号码类型必须是订单号，且必须选择参数才能预览,";
                    }
                    SmsSend ssBll = new SmsSend();
                    return ssBll.GetSmsContent(model.NumberCode, model.SmsContent, model.ParamsCode);
                }
                else if (model.TemplateType == "custom")
                {
                    return string.Format(model.SmsContent, model.ParamsValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    return model.SmsContent;
                }
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        [WebMethod]
        public string PreviewTemplate(SmsTemplateInfo model)
        {
            try
            {
                model.TemplateType = model.TemplateType.Trim();
                model.SmsContent = model.SmsContent.Trim();
                model.OrderCode = model.OrderCode.Trim();
                model.ParamsCode = model.ParamsCode.Trim();
                model.ParamsCode = model.ParamsCode.Trim();

                if (model.TemplateType == "auto")
                {
                    if (model.OrderCode == "")
                    {
                        return "订单号不能为空字符串，自动模板类型必须有订单号才能预览";
                    }
                }

                SmsTemplate smstBll = new SmsTemplate();
                return smstBll.GetTemplateContent(model.OrderCode, model);
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message + "";
            }
        }

        #endregion
    }
}
