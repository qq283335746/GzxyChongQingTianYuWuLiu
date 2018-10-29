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
    public class SmsTemplate : ISmsTemplate
    {
        #region 成员方法

        public int Insert(SmsTemplateInfo model)
        {
            if (model == null) return -1;

            string cmdText = @"insert into [Sms_Template] (UserId,Title,ParamsCode,ParamsName,ParamsValue,SmsContent,TemplateType,IsDefault,LastUpdatedDate) 
                             values (@UserId,@Title,@ParamsCode,@ParamsName,@ParamsValue,@SmsContent,@TemplateType,@IsDefault,@LastUpdatedDate)";

            SqlParameter[] parms = {
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@Title",SqlDbType.NVarChar,50), 
                                       new SqlParameter("@ParamsCode",SqlDbType.VarChar,256),
                                       new SqlParameter("@ParamsName",SqlDbType.NVarChar,256),
                                       new SqlParameter("@ParamsValue",SqlDbType.NVarChar,400),
                                       new SqlParameter("@SmsContent",SqlDbType.NVarChar,500),
                                       new SqlParameter("@TemplateType",SqlDbType.VarChar,20),
                                       new SqlParameter("@IsDefault",SqlDbType.Bit),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.UserId;
            parms[1].Value = model.Title;
            parms[2].Value = model.ParamsCode;
            parms[3].Value = model.ParamsName;
            parms[4].Value = model.ParamsValue;
            parms[5].Value = model.SmsContent;
            parms[6].Value = model.TemplateType;
            parms[7].Value = model.IsDefault;
            parms[8].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Update(SmsTemplateInfo model)
        {
            if (model == null) return -1;

            //定义查询命令
            string cmdText = @"update [Sms_Template] set UserId = @UserId,Title = @Title,ParamsCode = @ParamsCode,ParamsName = @ParamsName,
                             ParamsValue = @ParamsValue,
                             SmsContent = @SmsContent,TemplateType = @TemplateType,IsDefault = @IsDefault,LastUpdatedDate = @LastUpdatedDate
                             where Id = @Id";

            SqlParameter[] parms = {
                                       new SqlParameter("@Id",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@Title",SqlDbType.NVarChar,50), 
                                       new SqlParameter("@ParamsCode",SqlDbType.VarChar,256),
                                       new SqlParameter("@ParamsName",SqlDbType.NVarChar,256),
                                       new SqlParameter("@ParamsValue",SqlDbType.NVarChar,400),
                                       new SqlParameter("@SmsContent",SqlDbType.NVarChar,500),
                                       new SqlParameter("@TemplateType",SqlDbType.VarChar),
                                       new SqlParameter("@IsDefault",SqlDbType.Bit),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.UserId;
            parms[2].Value = model.Title;
            parms[3].Value = model.ParamsCode;
            parms[4].Value = model.ParamsName;
            parms[5].Value = model.ParamsValue;
            parms[6].Value = model.SmsContent;
            parms[7].Value = model.TemplateType;
            parms[8].Value = model.IsDefault;
            parms[9].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Delete(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return -1;

            string cmdText = "delete from Sms_Template where Id = @Id";
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
                sb.Append(@"delete from [Sms_Template] where Id = @Id" + n + " ;");
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

        public SmsTemplateInfo GetModel(object Id)
        {
            SmsTemplateInfo model = null;

            string cmdText = @"select top 1 Id,UserId,Title,ParamsCode,ParamsName,ParamsValue,SmsContent,TemplateType,IsDefault,LastUpdatedDate 
                            from [Sms_Template] where Id = @Id ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new SmsTemplateInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.Title = reader.GetString(2);
                        model.ParamsCode = reader.GetString(3);
                        model.ParamsName = reader.GetString(4);
                        model.ParamsValue = reader.GetString(5);
                        model.SmsContent = reader.GetString(6);
                        model.TemplateType = reader.GetString(7);
                        model.IsDefault = reader.GetBoolean(8);
                        model.LastUpdatedDate = reader.GetDateTime(9);
                    }
                }
            }

            return model;
        }

        public List<SmsTemplateInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Sms_Template] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalRecords = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
                       Id,UserId,Title,ParamsCode,ParamsName,ParamsValue,SmsContent,TemplateType,IsDefault,LastUpdatedDate
                      from [Sms_Template] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SmsTemplateInfo> list = new List<SmsTemplateInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsTemplateInfo model = new SmsTemplateInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.Title = reader.GetString(3);
                        model.ParamsCode = reader.GetString(4);
                        model.ParamsName = reader.GetString(5);
                        model.ParamsValue = reader.GetString(6);
                        model.SmsContent = reader.GetString(7);
                        model.TemplateType = reader.GetString(8);
                        model.IsDefault = reader.GetBoolean(9);
                        model.LastUpdatedDate = reader.GetDateTime(10);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<SmsTemplateInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
                             Id,UserId,Title,ParamsCode,ParamsName,ParamsValue,SmsContent,TemplateType,IsDefault,LastUpdatedDate
                             from [Sms_Template] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SmsTemplateInfo> list = new List<SmsTemplateInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsTemplateInfo model = new SmsTemplateInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.Title = reader.GetString(3);
                        model.ParamsCode = reader.GetString(4);
                        model.ParamsName = reader.GetString(5);
                        model.ParamsValue = reader.GetString(6);
                        model.SmsContent = reader.GetString(7);
                        model.TemplateType = reader.GetString(8);
                        model.IsDefault = reader.GetBoolean(9);
                        model.LastUpdatedDate = reader.GetDateTime(10);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<SmsTemplateInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            string cmdText = @"select Id,UserId,Title,ParamsCode,ParamsName,ParamsValue,SmsContent,TemplateType,IsDefault,LastUpdatedDate
                             from [Sms_Template] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;

            List<SmsTemplateInfo> list = new List<SmsTemplateInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SmsTemplateInfo model = new SmsTemplateInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.Title = reader.GetString(2);
                        model.ParamsCode = reader.GetString(3);
                        model.ParamsName = reader.GetString(4);
                        model.ParamsValue = reader.GetString(5);
                        model.SmsContent = reader.GetString(6);
                        model.TemplateType = reader.GetString(7);
                        model.IsDefault = reader.GetBoolean(8);
                        model.LastUpdatedDate = reader.GetDateTime(9);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public int SetDefault(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return -1;

            string cmdText = "update Sms_Template set IsDefault = 0 where Id <> @Id; ";
            cmdText += "update Sms_Template set IsDefault = 1 where Id = @Id; ";

            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText,parm);
        }

        public SmsTemplateInfo GetModelByEnumCode(string enumCode)
        {
            SmsTemplateInfo model = null;

            string cmdText = @"select top 1 smst.Id,smst.UserId,smst.Title,smst.ParamsCode,smst.ParamsName,smst.ParamsValue,smst.SmsContent,smst.TemplateType,smst.IsDefault,smst.LastUpdatedDate 
                            from [Sms_Template] smst join Sys_Enum se on se.EnumValue = smst.Title and se.EnumCode = @EnumCode ";
            SqlParameter parm = new SqlParameter("@EnumCode", enumCode);

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new SmsTemplateInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.Title = reader.GetString(2);
                        model.ParamsCode = reader.GetString(3);
                        model.ParamsName = reader.GetString(4);
                        model.ParamsValue = reader.GetString(5);
                        model.SmsContent = reader.GetString(6);
                        model.TemplateType = reader.GetString(7);
                        model.IsDefault = reader.GetBoolean(8);
                        model.LastUpdatedDate = reader.GetDateTime(9);
                    }
                }
            }

            return model;
        }

        #endregion
    }
}
