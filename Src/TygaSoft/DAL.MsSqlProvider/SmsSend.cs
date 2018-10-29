using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DBUtility;

namespace TygaSoft.DAL.MsSqlProvider
{
    public class SmsSend : ISmsSend
    {
        #region 成员方法

        public int Insert(SmsSendInfo model)
        {
            if (model == null) return -1;

            string cmdText = @"insert into [Sms_Send] (UserId,OrderCode,CarScanCode,TranNode,Receiver,MobilePhone,Customer,SmsContent,SendDate,SendStatus,LastUpdatedDate) 
                             values (@UserId,@OrderCode,@CarScanCode,@TranNode,@Receiver,@MobilePhone,@Customer,@SmsContent,@SendDate,@SendStatus,@LastUpdatedDate)";

            SqlParameter[] parms = {
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@CarScanCode",SqlDbType.VarChar,36),
                                       new SqlParameter("@TranNode",SqlDbType.NVarChar,20),
                                       new SqlParameter("@Receiver",SqlDbType.NVarChar,20),
                                       new SqlParameter("@MobilePhone",SqlDbType.VarChar,15),
                                       new SqlParameter("@Customer",SqlDbType.NVarChar,20),
                                       new SqlParameter("@SmsContent",SqlDbType.NVarChar,500),
                                       new SqlParameter("@SendDate",SqlDbType.DateTime),
                                       new SqlParameter("@SendStatus",SqlDbType.TinyInt),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.UserId;
            parms[1].Value = model.OrderCode;
            parms[2].Value = model.CarScanCode;
            parms[3].Value = model.TranNode;
            parms[4].Value = model.Receiver;
            parms[5].Value = model.MobilePhone;
            parms[6].Value = model.Customer;
            parms[7].Value = model.SmsContent;
            parms[8].Value = model.SendDate;
            parms[9].Value = model.SendStatus;
            parms[10].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Update(SmsSendInfo model)
        {
            if (model == null) return -1;

            //定义查询命令
            string cmdText = @"update [Sms_Send] set UserId = @UserId,OrderCode = @OrderCode,CarScanCode = @CarScanCode,TranNode = @TranNode,
                             Receiver = @Receiver,MobilePhone = @MobilePhone,Customer = @Customer,SmsContent = @SmsContent,SendDate = @SendDate,
                             SendStatus = @SendStatus,LastUpdatedDate = @LastUpdatedDate
                             where Id = @Id";

            SqlParameter[] parms = {
                                       new SqlParameter("@Id",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@CarScanCode",SqlDbType.VarChar,36),
                                       new SqlParameter("@TranNode",SqlDbType.NVarChar,20),
                                       new SqlParameter("@Receiver",SqlDbType.NVarChar,20),
                                       new SqlParameter("@MobilePhone",SqlDbType.VarChar,15),
                                       new SqlParameter("@Customer",SqlDbType.NVarChar,20),
                                       new SqlParameter("@SmsContent",SqlDbType.NVarChar,500),
                                       new SqlParameter("@SendDate",SqlDbType.DateTime),
                                       new SqlParameter("@SendStatus",SqlDbType.TinyInt),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.UserId;
            parms[2].Value = model.OrderCode;
            parms[3].Value = model.CarScanCode;
            parms[4].Value = model.TranNode;
            parms[5].Value = model.Receiver;
            parms[6].Value = model.MobilePhone;
            parms[7].Value = model.Customer;
            parms[8].Value = model.SmsContent;
            parms[9].Value = model.SendDate;
            parms[10].Value = model.SendStatus;
            parms[11].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Delete(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return -1;

            string cmdText = "delete from Sms_Send where Id = @Id";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm);
        }

        public bool DeleteBatch(IList<object> list)
        {
            if (list == null || list.Count == 0) return false;

            bool result = false;
            StringBuilder sb = new StringBuilder();
            ParamsHelper parms = new ParamsHelper();
            int n = 0;
            foreach (string item in list)
            {
                n++;
                sb.Append(@"delete from [Sms_Send] where Id = @Id" + n + " ;");
                SqlParameter parm = new SqlParameter("@Id" + n + "", SqlDbType.UniqueIdentifier);
                parm.Value = Guid.Parse(item);
                parms.Add(parm);
            }
            using (SqlConnection conn = new SqlConnection(SqlHelper.SqlProviderConnString))
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int effect = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sb.ToString(), parms != null ? parms.ToArray() : null);
                        tran.Commit();
                        if (effect > 0) result = true;
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return result;
        }

        public SmsSendInfo GetModel(object Id)
        {
            SmsSendInfo model = null;

            string cmdText = @"select top 1 Id,UserId,OrderCode,CarScanCode,TranNode,Receiver,MobilePhone,Customer,SmsContent,
                            SendDate,SendStatus,LastUpdatedDate 
                            from [Sms_Send] where Id = @Id ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new SmsSendInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.OrderCode = reader.GetString(2);
                        model.CarScanCode = reader.GetString(3);
                        model.TranNode = reader.GetString(4);
                        model.Receiver = reader.GetString(5);
                        model.MobilePhone = reader.GetString(6);
                        model.Customer = reader.GetString(7);
                        model.SmsContent = reader.GetString(8);
                        model.SendDate = reader.GetDateTime(9);
                        model.SendStatus = reader.GetInt16(10);
                        model.LastUpdatedDate = reader.GetDateTime(11);
                    }
                }
            }

            return model;
        }

        public List<SmsSendInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Sms_Send] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalRecords = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
                       Id,UserId,OrderCode,CarScanCode,TranNode,Receiver,MobilePhone,Customer,SmsContent,
                       SendDate,SendStatus,LastUpdatedDate
                      from [Sms_Send] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SmsSendInfo> list = new List<SmsSendInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsSendInfo model = new SmsSendInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.OrderCode = reader.GetString(3);
                        model.CarScanCode = reader.GetString(4);
                        model.TranNode = reader.GetString(5);
                        model.Receiver = reader.GetString(6);
                        model.MobilePhone = reader.GetString(7);
                        model.Customer = reader.GetString(8);
                        model.SmsContent = reader.GetString(9);
                        model.SendDate = reader.GetDateTime(10);
                        model.SendStatus = reader.GetByte(11);
                        model.LastUpdatedDate = reader.GetDateTime(12);
                        model.SendStatusText = "待发出";
                        switch (model.SendStatus)
                        {
                            case 1:
                                model.SendStatusText = "已发出";
                                break;
                            case 2:
                                model.SendStatusText = "发送失败";
                                break;
                            default:
                                break;
                        }

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<SmsSendInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
                             Id,UserId,OrderCode,CarScanCode,TranNode,Receiver,MobilePhone,Customer,SmsContent,
                             SendDate,SendStatus,LastUpdatedDate
                             from [Sms_Send] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SmsSendInfo> list = new List<SmsSendInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsSendInfo model = new SmsSendInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.OrderCode = reader.GetString(3);
                        model.CarScanCode = reader.GetString(4);
                        model.TranNode = reader.GetString(5);
                        model.Receiver = reader.GetString(6);
                        model.MobilePhone = reader.GetString(7);
                        model.Customer = reader.GetString(8);
                        model.SmsContent = reader.GetString(9);
                        model.SendDate = reader.GetDateTime(10);
                        model.SendStatus = reader.GetInt16(11);
                        model.LastUpdatedDate = reader.GetDateTime(12);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<SmsSendInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            string cmdText = @"select Id,UserId,OrderCode,CarScanCode,TranNode,Receiver,MobilePhone,Customer,SmsContent,
                             SendDate,SendStatus,LastUpdatedDate
                             from [Sms_Send] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;

            List<SmsSendInfo> list = new List<SmsSendInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsSendInfo model = new SmsSendInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.OrderCode = reader.GetString(2);
                        model.CarScanCode = reader.GetString(3);
                        model.TranNode = reader.GetString(4);
                        model.Receiver = reader.GetString(5);
                        model.MobilePhone = reader.GetString(6);
                        model.Customer = reader.GetString(7);
                        model.SmsContent = reader.GetString(8);
                        model.SendDate = reader.GetDateTime(9);
                        model.SendStatus = reader.GetInt16(10);
                        model.LastUpdatedDate = reader.GetDateTime(11);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public DataSet GetSmsParams(string orderCode)
        {
            string cmdText = @"select top 1 o.code OrderCode,o.BusinessDate DealDate,CONVERT(varchar(100), o.BusinessDate, 23) DealDateNoTime,smo.Name OrgName,
                            c.Name Customer,cmp.Name SendProduct,bst1.Name FromStation,bst2.Name ToStation,
                            brs1.Name Sender,o.Sendercontact SenderContact,o.Sendercellphone SenderMobilePhone,brs2.Name Receiver,o.Receivercontact ReceiverContact,
                            o.Receivercellphone ReceiverMobilePhone,o.TotalPackageCount TotalPackage,adc.Code CarScanCode
                            from LM_Order o
                            left join SM_Organization smo on smo.ID = o.OrganizationID
                            left join Base_Customer c on c.ID = o.CustomerID 
                            left join CM_Product cmp on cmp.ID = o.LogisticsProductID
                            left join Base_Station bst1 on bst1.ID = o.FromStationID
                            left join Base_Station bst2 on bst2.ID = o.ToStationID
                            left join Base_ReceiverSender brs1 on brs1.ID = o.SenderID
                            left join Base_ReceiverSender brs2 on brs2.ID = o.ReceiverID
                            left join TG_Task tgt on tgt.OrderID = o.ID
                            left join TG_ADCOrderGoods adcog on adcog.TaskID = tgt.ID
                            left join TM_ADC adc on adc.ID = adcog.ADCID
                            where o.Code = @OrderCode
                            group by o.code,o.BusinessDate,smo.Name,c.Name,cmp.Name,bst1.Name,
                             bst2.Name,brs1.Name,o.Sendercontact,o.Sendercellphone,brs2.Name,
                            o.Receivercontact,o.Receivercellphone,o.TotalPackageCount,adc.Code";

            SqlParameter parm = new SqlParameter(@"OrderCode", SqlDbType.VarChar, 36);
            parm.Value = orderCode;

            return SqlHelper.ExecuteDataset(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText, parm);
        }

        #endregion
    }
}
