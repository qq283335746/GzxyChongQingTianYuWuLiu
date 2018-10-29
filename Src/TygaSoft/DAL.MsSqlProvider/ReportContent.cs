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
    public class ReportContent : IReportContent
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(ReportContentInfo model)
        {
            if (model == null) return -1;

            string cmdText = "insert into [ReportContent] (UserId,FromUrl,FromType,GiveName,FromDate) values (@UserId,@FromUrl,@FromType,@GiveName,@FromDate)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@FromUrl",SqlDbType.NVarChar,300), 
                                     new SqlParameter("@FromType",SqlDbType.NVarChar,36),
                                     new SqlParameter("@GiveName",SqlDbType.NVarChar,256),
                                     new SqlParameter("@FromDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.UserId;
            parms[1].Value = model.FromUrl;
            parms[2].Value = model.FromType;
            parms[3].Value = model.GiveName;
            parms[4].Value = model.FromDate;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ReportContentInfo model)
        {
            if (model == null) return -1;

            //定义查询命令
            string cmdText = @"update [ReportContent] set UserId = @UserId,FromUrl = @FromUrl,FromType = @FromType,GiveName = @GiveName,FromDate = @FromDate where Id = @Id";

            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@UserId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@FromUrl",SqlDbType.NVarChar,300),
                                     new SqlParameter("@FromType",SqlDbType.NVarChar,36),
                                     new SqlParameter("@GiveName",SqlDbType.NVarChar,256),
                                     new SqlParameter("@FromDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = Guid.Parse(model.UserId.ToString());
            parms[2].Value = model.FromUrl;
            parms[3].Value = model.FromType;
            parms[4].Value = model.GiveName;
            parms[5].Value = model.FromDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return -1;

            string cmdText = "delete from ReportContent where Id = @Id";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm);
        }

        /// <summary>
        /// 批量删除数据（启用事务）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
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
                sb.Append(@"delete from [ReportContent] where Id = @Id" + n + " ;");
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

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ReportContentInfo GetModel(object Id)
        {
            ReportContentInfo model = null;

            string cmdText = @"select top 1 Id,UserId,FromUrl,FromType,GiveName,FromDate from [ReportContent] where Id = @Id ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new ReportContentInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.FromUrl = reader["FromUrl"].ToString();
                        model.FromType = reader["FromType"].ToString();
                        model.GiveName = reader["GiveName"].ToString();
                        model.FromDate = DateTime.Parse(reader["FromDate"].ToString());
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<ReportContentInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [ReportContent] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.UserId,t1.FromUrl,t1.FromType,t1.GiveName,t1.FromDate,u.UserName from [ReportContent] t1 
                      left join Aspnet_Users u on u.UserId = t1.UserId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ReportContentInfo> list = new List<ReportContentInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReportContentInfo model = new ReportContentInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.FromUrl = reader["FromUrl"].ToString();
                        model.FromType = reader["FromType"].ToString();
                        model.GiveName = reader["GiveName"].ToString();
                        model.FromDate = DateTime.Parse(reader["FromDate"].ToString());

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<ReportContentInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.UserId,t1.FromUrl,t1.FromType,t1.GiveName,t1.FromDate,u.UserName from [ReportContent] t1 
                             left join Aspnet_Users u on u.UserId = t1.UserId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ReportContentInfo> list = new List<ReportContentInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReportContentInfo model = new ReportContentInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.FromUrl = reader["FromUrl"].ToString();
                        model.FromType = reader["FromType"].ToString();
                        model.GiveName = reader["GiveName"].ToString();
                        model.FromDate = DateTime.Parse(reader["FromDate"].ToString());

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        public List<ReportContentInfo> GetList()
        {
            string cmdText = @"select * from [ReportContent] ";

            List<ReportContentInfo> list = new List<ReportContentInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReportContentInfo model = new ReportContentInfo();
                        model.Id = reader["Id"];
                        model.UserId = reader["UserId"];
                        model.FromUrl = reader["FromUrl"].ToString();
                        model.FromType = reader["FromType"].ToString();
                        model.GiveName = reader["GiveName"].ToString();
                        model.FromDate = DateTime.Parse(reader["FromDate"].ToString());

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
