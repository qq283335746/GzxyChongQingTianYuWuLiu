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
    public class SitePoint
    {
        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SitePointInfo model)
        {
            if (model == null) return -1;

            //判断当前记录是否存在，如果存在则返回;
            if (IsExist(model.PointName, Guid.Empty)) return 110;

            string cmdText = "insert into [SitePoint] (PointName,PointNum,Remark) values (@PointName,@PointNum,@Remark)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@PointName",SqlDbType.NVarChar,50), 
                                     new SqlParameter("@PointNum",SqlDbType.Decimal), 
                                     new SqlParameter("@Remark",SqlDbType.NVarChar,300)
                                   };
            parms[0].Value = model.PointName;
            parms[1].Value = model.PointNum;
            parms[2].Value = model.Remark;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SitePointInfo model)
        {
            if (model == null) return -1;

            if (IsExist(model.PointName, model.Id)) return 110;

            //定义查询命令
            string cmdText = @"update [SitePoint] set PointName = @PointName,PointNum = @PointNum,Remark = @Remark where Id = @Id";

            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@PointName",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@PointNum",SqlDbType.Decimal), 
                                     new SqlParameter("@Remark",SqlDbType.NVarChar,300)
                                   };
            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = model.PointName;
            parms[2].Value = model.PointNum;
            parms[3].Value = model.Remark;

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

            string cmdText = "delete from SitePoint where Id = @Id";
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
                sb.Append(@"delete from [SitePoint] where Id = @Id" + n + " ;");
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
        public SitePointInfo GetModel(object Id)
        {
            SitePointInfo model = null;

            string cmdText = @"select top 1 Id,PointName,PointNum,Remark from [SitePoint] where Id = @Id ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new SitePointInfo();
                        model.Id = reader["Id"];
                        model.PointName = reader["PointName"].ToString();
                        model.PointNum = decimal.Parse(reader["PointNum"].ToString());
                        model.Remark = reader["Remark"].ToString();
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 是否存在对应数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool IsExist(string name, object Id)
        {
            bool isExist = false;

            Guid gId = Guid.Empty;
            if (!Guid.TryParse(Id.ToString(), out gId))
            {
                throw new ArgumentException("参数值无效", "Id");
            }

            ParamsHelper parms = new ParamsHelper();

            string cmdText = "select 1 from [SitePoint] where PointName = @PointName ";
            if (!gId.Equals(Guid.Empty))
            {
                cmdText = "select 1 from [SitePoint] where PointName = @PointName and Id <> @Id ";
                SqlParameter parm1 = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
                parm1.Value = gId;
                parms.Add(parm1);
            }
            SqlParameter parm = new SqlParameter("@PointName", SqlDbType.NVarChar, 50);
            parm.Value = name;
            parms.Add(parm);

            object obj = SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms.ToArray());
            if (obj != null) isExist = true;

            return isExist;
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
        public List<SitePointInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [SitePoint] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.PointName,t1.PointNum,t1.Remark from [SitePoint] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SitePointInfo> list = new List<SitePointInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SitePointInfo model = new SitePointInfo();
                        model.Id = reader["Id"];
                        model.PointName = reader["PointName"].ToString();
                        model.PointNum = decimal.Parse(reader["PointNum"].ToString());
                        model.Remark = reader["Remark"].ToString();

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
        public List<SitePointInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.PointName,t1.PointNum,t1.Remark from [SitePoint] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SitePointInfo> list = new List<SitePointInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SitePointInfo model = new SitePointInfo();
                        model.Id = reader["Id"];
                        model.PointName = reader["PointName"].ToString();
                        model.PointNum = decimal.Parse(reader["PointNum"].ToString());
                        model.Remark = reader["Remark"].ToString();

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
        public List<SitePointInfo> GetList()
        {
            string cmdText = @"select * from [SitePoint] ";

            List<SitePointInfo> list = new List<SitePointInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SitePointInfo model = new SitePointInfo();
                        model.Id = reader["Id"];
                        model.PointName = reader["PointName"].ToString();
                        model.PointNum = decimal.Parse(reader["PointNum"].ToString());
                        model.Remark = reader["Remark"].ToString();

                        list.Add(model);
                    }
                }
            }

            return list;
        }
    }
}
