using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using TygaSoft.Model;
using TygaSoft.BLL;

namespace TygaSoft.WCFService
{
    public class PDAOrderService : IPDAOrder
    {
        #region 成员方法

        /// <summary>
        /// PDA测试网络连接
        /// </summary>
        /// <returns></returns>
        public string HelloWord()
        {
            return "1";
        }

        /// <summary>
        /// 新增PDA扫描数据
        /// </summary>
        /// <param name="opType">操作类型</param>
        /// <param name="barCode">单号条码</param>
        /// <param name="scanTime">扫描时间</param>
        /// <param name="userName">用户名</param>
        /// <returns>返回：包含“成功”，则调用成功，否则返回调用失败原因</returns>
        public string Insert(string opType, string barCode, DateTime scanTime, string userName)
        {
            #region 参数合法性检查

            if (string.IsNullOrEmpty(opType))
            {
                return "操作类型不能为空";
            }
            opType = opType.Trim();
            if (string.IsNullOrEmpty(barCode))
            {
                return "单号条码不能为空";
            }
            barCode = barCode.Trim();
            //if (barCode.Length != 13)
            //{
            //    return "单号条码不正确";
            //}

            if (scanTime == DateTime.MinValue)
            {
                return "扫描时间格式不正确";
            }

            if (string.IsNullOrEmpty(userName))
            {
                return "用户不能为空";
            }

            #endregion

            try
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user == null)
                {
                    return "不存在用户：" + userName;
                }

                SysEnum seBll = new SysEnum();
                var seList = seBll.GetList("and t2.EnumCode = 'SendReceiveType'", null);
                if (seList == null || seList.Count == 0)
                {
                    return "服务端的操作类型未设定，请先完成设定";
                }

                var firstModel = seList.OrderBy(m => m.Sort).First();
                var currSeModel = seList.Find(m => m.EnumValue.Trim() == opType);
                if (currSeModel == null)
                {
                    return "当前操作类型“" + opType + "”不存在";
                }

                object nextNodeId = Guid.Empty;
                var nextSeModel = seList.FindAll(m => m.Sort > currSeModel.Sort).OrderBy(m => m.Sort).FirstOrDefault();
                if (nextSeModel == null) nextNodeId = currSeModel.Id;
                else nextNodeId = nextSeModel.Id;

                bool isOsdExists = false;

                PDAOrderScanInfo currOsModel = new PDAOrderScanInfo();
                currOsModel.OrderCode = barCode;
                currOsModel.CurrNodeId = currSeModel.Id;
                currOsModel.NextNodeId = nextNodeId;
                currOsModel.IsFinish = false;
                currOsModel.LastUpdatedDate = DateTime.Now;

                PDAOrderScanDetailInfo currOsdModel = new PDAOrderScanDetailInfo();
                currOsdModel.OrderCode = barCode;
                currOsdModel.CurrNodeId = currOsModel.CurrNodeId;
                currOsdModel.ScanTime = scanTime;
                currOsdModel.UserId = user.ProviderUserKey;
                currOsdModel.LastUpdatedDate = currOsModel.LastUpdatedDate;

                Order oBll = new Order();
                PDAOrderScan osBll = new PDAOrderScan();
                PDAOrderScanDetail osdBll = new PDAOrderScanDetail();
                TyUser tyuBll = new TyUser();
                SmsSend smsBll = new SmsSend();

                string sRemark = "";
                TyUserInfo tyuModel = tyuBll.GetModelByUser(user.UserName);
                if (tyuModel != null && !string.IsNullOrWhiteSpace(tyuModel.OrganizationName))
                {
                    sRemark = string.Format(currSeModel.Remark, tyuModel.OrganizationName);
                }
                else sRemark = currSeModel.Remark.Replace(@"{0}", "");
                sRemark = currSeModel.EnumValue + "：" + sRemark;

                currOsdModel.Remark = sRemark;

                if (opType == "干线发运" || opType == "干线到达")
                {
                    var orders = oBll.GetOrderByCarcode(barCode);
                    if (orders == null || orders.Count() == 0)
                    {
                        return "操作类型：" + barCode + "找不到订单号，有错误";
                    }

                    foreach (var currOrderCode in orders)
                    {
                        currOsModel.OrderCode = currOrderCode;
                        currOsdModel.OrderCode = currOrderCode;

                        var oldOsdList = osdBll.GetList(currOrderCode);
                        if (oldOsdList != null)
                        {
                            isOsdExists = oldOsdList.Exists(m => m.CurrNodeId.Equals(currSeModel.Id));
                        }

                        if (isOsdExists)
                        {
                            return "订单号：" + currOrderCode + " 操作类型：" + opType + "已存在，不能重复提交";
                        }

                        //发短信
                        SmsSendInfo ssiModel = new SmsSendInfo();
                        ssiModel.OrderCode = currOrderCode;
                        ssiModel.TranNode = currSeModel.EnumCode;
                        ssiModel.TranNodeText = currSeModel.EnumValue;

                        using (TransactionScope scope = new TransactionScope())
                        {
                            if (osBll.GetModel(currOrderCode) == null)
                            {
                                osBll.Insert(currOsModel);
                            }
                            else
                            {
                                osBll.Update(currOsModel);
                            }

                            osdBll.Insert(currOsdModel);

                            scope.Complete();
                        }

                        smsBll.InsertByStrategy(ssiModel);
                    }
                }

                else if (currSeModel.Sort == firstModel.Sort)
                {
                    var oldOsModel = osBll.GetModel(barCode);
                    if (oldOsModel != null)
                    {
                        return "订单号：" + barCode + ",操作类型：" + opType + "已存在，不能重复提交";
                    }

                    SmsSendInfo ssiModel = new SmsSendInfo();
                    ssiModel.OrderCode = barCode;
                    ssiModel.TranNode = currSeModel.EnumCode;
                    ssiModel.TranNodeText = currSeModel.EnumValue;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        osBll.Insert(currOsModel);
                        osdBll.Insert(currOsdModel);

                        scope.Complete();
                    }

                    smsBll.InsertByStrategy(ssiModel);
                }
                else
                {
                    var oldOsModel = osBll.GetModel(barCode);
                    if (oldOsModel == null)
                    {
                        return "订单号：" + barCode + ",操作类型：" + opType + "未取货，有错误";
                    }

                    var oldOsdList = osdBll.GetList(barCode);
                    if (oldOsdList != null)
                    {
                        isOsdExists = oldOsdList.Exists(m => m.CurrNodeId.Equals(currSeModel.Id));

                        currOsModel.IsFinish = oldOsdList.Count == 5;
                    }

                    if (currOsModel.IsFinish)
                    {
                        return "订单号：" + barCode + "已完成所有操作";
                    }

                    if (isOsdExists)
                    {
                        return "订单号：" + barCode + ",操作类型：" + opType + "已存在，不能重复提交";
                    }

                    SmsSendInfo ssiModel = new SmsSendInfo();
                    ssiModel.OrderCode = barCode;
                    ssiModel.TranNode = currSeModel.EnumCode;
                    ssiModel.TranNodeText = currSeModel.EnumValue;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        osBll.Update(currOsModel);

                        osdBll.Insert(currOsdModel);

                        scope.Complete();
                    }

                    smsBll.InsertByStrategy(ssiModel);
                }

                Console.WriteLine("request: opType：{0}，barCode：{1}，scanTime：{3}，user：{2}", opType, barCode, scanTime, userName);

                return "保存成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 批量提交PDA扫描数据
        /// </summary>
        /// <param name="dt">应包含opType、barCode、scanTime、userName等列</param>
        /// <returns>返回：包含“成功”，则调用成功，否则返回调用失败原因</returns>
        public string InsertByBatch(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return "无任何可保存的数据";

            string[] colNames = { "opType", "barCode", "scanTime", "userName" };

            DataColumnCollection cols = dt.Columns;
            foreach (DataColumn col in cols)
            {
                if (!colNames.Contains(col.ColumnName))
                {
                    return "检测到提交的数据集中未包含" + col.ColumnName + "列";
                }
            }

            DataRowCollection rows = dt.Rows;

            List<PDAOrderScanDetailInfo> osdList = new List<PDAOrderScanDetailInfo>();

            //定义非重复的用户名、用户ID
            Dictionary<string, object> dicUser = new Dictionary<string, object>();

            try
            {
                SysEnum seBll = new SysEnum();
                var seList = seBll.GetList("and t2.EnumCode = 'SendReceiveType'", null);
                if (seList == null || seList.Count == 0)
                {
                    return "服务端的操作类型未设定，请先完成设定";
                }
                var firstModel = seList.OrderBy(m => m.Sort).First();

                foreach (DataRow row in rows)
                {
                    string opType = "";
                    if (row["opType"] != DBNull.Value)
                    {
                        opType = row["opType"].ToString();
                    }
                    else
                    {
                        return "操作类型不能为空";
                    }

                    var seModel = seList.Find(m => m.EnumValue.Trim() == opType.Trim());
                    if (seModel == null) return "不存在操作类型：" + opType;

                    string barCode = "";
                    if (row["barCode"] != DBNull.Value)
                    {
                        barCode = row["barCode"].ToString();
                    }
                    DateTime scanTime = DateTime.MinValue;
                    if (row["scanTime"] != DBNull.Value)
                    {
                        DateTime.TryParse(row["scanTime"].ToString(), out scanTime);
                    }
                    if (scanTime == DateTime.MinValue)
                    {
                        return "数据集中包含不合法数据：scanTime值格式不正确";
                    }

                    string userName = "";
                    if (row["userName"] != DBNull.Value)
                    {
                        userName = row["userName"].ToString();
                    }
                    if (string.IsNullOrEmpty(userName))
                    {
                        return "数据集中包含用户名为空";
                    }

                    if (!dicUser.ContainsKey(userName)) dicUser.Add(userName, Guid.Empty);

                    osdList.Add(new PDAOrderScanDetailInfo { OrderCode = barCode, CurrNodeId = seModel.Id, ScanTime = scanTime, Remark = seModel.Remark, Sort = seModel.Sort, LastUpdatedDate = DateTime.Now, UserName = userName, SysEnumValue = opType });
                }

                TyUser tyuBll = new TyUser();

                foreach (KeyValuePair<string,object> kvp in dicUser)
                {
                    MembershipUser user = Membership.GetUser(kvp.Key, false);
                    if (user == null) return "不存在用户：" + kvp.Key;

                    TyUserInfo tyuModel = tyuBll.GetModelByUser(user.UserName);

                    var currList = osdList.FindAll(m => m.UserName == kvp.Key);
                    foreach (var item in currList)
                    {
                        item.UserId = user.ProviderUserKey;

                        string sRemark = "";
                        if (tyuModel != null && !string.IsNullOrWhiteSpace(tyuModel.OrganizationName))
                        {
                            sRemark = string.Format(item.Remark, tyuModel.OrganizationName);
                        }
                        else sRemark = item.Remark.Replace(@"{0}", "");
                        sRemark = item.SysEnumValue + "：" + sRemark;

                        item.Remark = sRemark;
                    }
                }

                PDAOrderScan osBll = new PDAOrderScan();
                PDAOrderScanDetail osdBll = new PDAOrderScanDetail();
                Order oBll = new Order();
                SmsSend smsBll = new SmsSend();

                var q = osdList.OrderBy(m => m.Sort);
                foreach (var item in q)
                {
                    object nextNodeId = Guid.Empty;
                    var currSeModel = seList.Find(m => m.EnumValue.Trim() == item.SysEnumValue);
                    var nextSeModel = seList.FindAll(m => m.Sort > item.Sort).OrderBy(m => m.Sort).FirstOrDefault();
                    if (nextSeModel == null) nextNodeId = item.CurrNodeId;
                    else nextNodeId = nextSeModel.Id;
                    var lastSeModel = seList.OrderByDescending(m => m.Sort).First();
                    bool isFinish = lastSeModel.EnumValue.Trim() == item.SysEnumValue;

                    bool isOsdExists = false;

                    PDAOrderScanInfo currOsModel = new PDAOrderScanInfo();
                    currOsModel.OrderCode = item.OrderCode;
                    currOsModel.CurrNodeId = item.CurrNodeId;
                    currOsModel.NextNodeId = nextNodeId;
                    currOsModel.IsFinish = isFinish;
                    currOsModel.LastUpdatedDate = DateTime.Now;

                    PDAOrderScanDetailInfo currOsdModel = new PDAOrderScanDetailInfo();
                    currOsdModel.OrderCode = item.OrderCode;
                    currOsdModel.CurrNodeId = item.CurrNodeId;
                    currOsdModel.ScanTime = item.ScanTime;
                    currOsdModel.UserId = item.UserId;
                    currOsdModel.LastUpdatedDate = currOsModel.LastUpdatedDate;
                    currOsdModel.Remark = item.Remark;

                    if (item.SysEnumValue == "干线发运" || item.SysEnumValue == "干线到达")
                    {
                        var orders = oBll.GetOrderByCarcode(item.OrderCode);
                        if (orders == null || orders.Count() == 0)
                        {
                            return "操作类型：" + item.SysEnumValue + "找不到订单号，有错误";
                        }

                        foreach (var currOrderCode in orders)
                        {
                            currOsModel.OrderCode = currOrderCode;
                            currOsdModel.OrderCode = currOrderCode;

                            var oldOsdList = osdBll.GetList(currOrderCode);
                            if (oldOsdList != null)
                            {
                                isOsdExists = oldOsdList.Exists(m => m.CurrNodeId.Equals(item.CurrNodeId));
                            }

                            if (!isOsdExists)
                            {
                                SmsSendInfo ssiModel = new SmsSendInfo();
                                ssiModel.OrderCode = currOrderCode;
                                ssiModel.TranNode = currSeModel.EnumCode;
                                ssiModel.TranNodeText = currSeModel.EnumValue;

                                using (TransactionScope scope = new TransactionScope())
                                {
                                    if (osBll.GetModel(currOrderCode) == null)
                                    {
                                        osBll.Insert(currOsModel);
                                    }
                                    else
                                    {
                                        osBll.Update(currOsModel);
                                    }

                                    osdBll.Insert(currOsdModel);

                                    scope.Complete();
                                }

                                smsBll.InsertByStrategy(ssiModel);
                            } 
                        }
                    }
                    else if (item.Sort == firstModel.Sort)
                    {
                        var oldOsModel = osBll.GetModel(item.OrderCode);
                        if (oldOsModel == null)
                        {
                            SmsSendInfo ssiModel = new SmsSendInfo();
                            ssiModel.OrderCode = item.OrderCode;
                            ssiModel.TranNode = currSeModel.EnumCode;
                            ssiModel.TranNodeText = currSeModel.EnumValue;

                            using (TransactionScope scope = new TransactionScope())
                            {
                                osBll.Insert(currOsModel);
                                osdBll.Insert(currOsdModel);

                                scope.Complete();
                            }

                            smsBll.InsertByStrategy(ssiModel);
                        }
                    }
                    else
                    {
                        var oldOsModel = osBll.GetModel(item.OrderCode);
                        if (oldOsModel == null)
                        {
                            return "订单号：" + item.OrderCode + ",操作类型：" + item.SysEnumValue + "未取货，有错误";
                        }

                        var oldOsdList = osdBll.GetList(item.OrderCode);
                        if (oldOsdList != null)
                        {
                            isOsdExists = oldOsdList.Exists(m => m.CurrNodeId.Equals(item.CurrNodeId));

                            currOsModel.IsFinish = oldOsdList.Count == 5;
                        }

                        if (currOsModel.IsFinish) continue;

                        if (!isOsdExists)
                        {
                            SmsSendInfo ssiModel = new SmsSendInfo();
                            ssiModel.OrderCode = currOsModel.OrderCode;
                            ssiModel.TranNode = currSeModel.EnumCode;
                            ssiModel.TranNodeText = currSeModel.EnumValue;

                            using (TransactionScope scope = new TransactionScope())
                            {
                                osBll.Update(currOsModel);

                                osdBll.Insert(currOsdModel);

                                scope.Complete();
                            }

                            smsBll.InsertByStrategy(ssiModel);
                        }
                    }

                    Console.WriteLine("request: opType：{0}，barCode：{1}，scanTime：{3}，user：{2}", item.SysEnumValue, item.OrderCode, item.ScanTime, item.UserName);

                }

                return "保存成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取所有有效的用户信息
        /// </summary>
        /// <returns></returns>
        public string[] GetAllUsers()
        {
            return Roles.GetUsersInRole("PDA_Admin");
        }

        /// <summary>
        /// 验证提供的用户名和密码是有效的。
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string ValidateUser(string username, string password)
        {
            try
            {
                MembershipUser userInfo = Membership.GetUser(username);
                if (!Membership.ValidateUser(username, password))
                {
                    if (userInfo == null)
                    {
                        return "不存在用户：" + username;
                    }
                    if (userInfo.IsLockedOut)
                    {
                        return "您的账号已被锁定，请联系管理员先解锁后才能登录！";
                    }
                    if (!userInfo.IsApproved)
                    {
                        return "您的帐户尚未获得批准。您无法登录，直到管理员批准您的帐户！";
                    }
                    else
                    {
                        return "密码不正确，请检查！";
                    }
                }

                return "登录成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}
