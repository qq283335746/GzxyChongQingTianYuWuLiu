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
    /// <summary>
    /// 同步用户标识表
    /// </summary>
    public class TyUser : ITyUser
    {
        #region 成员方法

        public int Insert(TyUserInfo model)
        {
            if (model == null) return -1;

            string cmdText = "insert into [TyUserProcessor] (UserName,Password,IsEnable,LastUpdatedDate) values (@UserName,@Password,@IsEnable,@LastUpdatedDate)";

            SqlParameter[] parms = {
                                       new SqlParameter("@UserName",SqlDbType.NVarChar,50), 
                                       new SqlParameter("@Password",SqlDbType.VarChar,6),
                                       new SqlParameter("@IsEnable",SqlDbType.Bit), 
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.UserName;
            parms[1].Value = model.Password;
            parms[2].Value = model.IsEnable;
            parms[3].Value = model.LastUpdatedDate;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Update(TyUserInfo model)
        {
            throw new NotImplementedException();
        }

        public int Delete(object Id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBatch(IList<object> list)
        {
            throw new NotImplementedException();
        }

        public TyUserInfo GetModel(object Id)
        {
            throw new NotImplementedException();
        }

        public TyUserInfo GetModelByUser(string userName)
        {
            TyUserInfo model = null;

            string cmdText = @"select top 1 u.Code UserName, smo.Name OrganizationName from SM_User u  
                            left join dbo.SM_Organization smo on smo.ID = u.OrganizationID
                            where u.Code = @UserName ";
            SqlParameter parm = new SqlParameter("@UserName", SqlDbType.NVarChar,50);
            parm.Value = userName;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new TyUserInfo();
                        model.UserName = reader.GetString(0);
                        model.OrganizationName = reader.GetString(1);
                    }
                }
            }

            return model;
        }


        public List<TyUserInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [TyUserProcessor] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,UserName,Password,IsEnable,LastUpdatedDate
                      from [TyUserProcessor] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<TyUserInfo> list = new List<TyUserInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TyUserInfo model = new TyUserInfo();
                        model.UserName = reader.GetString(1).Trim();
                        model.Password = reader.GetString(2).Trim(); ;
                        model.IsEnable = reader.GetBoolean(3);
                        model.LastUpdatedDate = reader.GetDateTime(4);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<TyUserInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            throw new NotImplementedException();
        }

        public List<TyUserInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            throw new NotImplementedException();
        }

        public List<TyUserInfo> GetList()
        {
            string cmdText = @"select UserName,Password,IsEnable,LastUpdatedDate
                             from [TyUserProcessor] ";

            List<TyUserInfo> list = new List<TyUserInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TyUserInfo model = new TyUserInfo();
                        model.UserName = reader.GetString(0).Trim();
                        model.Password = reader.GetString(1).Trim();
                        model.IsEnable = reader.GetBoolean(2);
                        model.LastUpdatedDate = reader.GetDateTime(3);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public string[] GetTyUsers()
        {
            string itemAppend = "";

            string cmdText = @"select Code from SM_USER where IsUsed='1' ";

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        itemAppend += reader.GetString(0).Trim() + ",";
                    }
                }
            }

            itemAppend = itemAppend.Trim(',');

            return itemAppend.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
        }

        public DataSet GetAllUsers()
        {
            string cmdText = @"select Code from SM_USER where IsUsed='1' order by Code";

            return SqlHelper.ExecuteDataset(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText);
        }

        public string ValidateUser(string username, string password)
        {
            //string cmdText = @"select * from SM_USER where Code = @UserName ";
            //SqlParameter parm = new SqlParameter("@UserName", SqlDbType.NVarChar);

            //using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText))
            //{
            //    if (reader == null || !reader.HasRows)
            //    {
            //        return "不存在用户:" + username;
            //    }

            //}

            return "";
        }

        public int UpdateEnable(string userName, bool isEnable)
        {
            string cmdText = @"update TyUserProcessor set IsEnable = @IsEnable,LastUpdatedDate = GETDATE() where UserName = @UserName";

            SqlParameter[] parms = {
                                       new SqlParameter("@UserName",SqlDbType.NVarChar,50), 
                                       new SqlParameter("@IsEnable",SqlDbType.Bit)
                                   };
            parms[0].Value = userName;
            parms[1].Value = isEnable;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        #endregion

    }
}
