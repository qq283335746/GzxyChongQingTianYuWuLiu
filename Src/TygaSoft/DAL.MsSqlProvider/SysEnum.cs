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
    public class SysEnum : ISysEnum
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysEnumInfo model)
        {
            if (model == null) return -1;

            //判断当前记录是否存在，如果存在则返回;
            if (IsExist(model.EnumName, model.ParentId, null)) return 110;

            string cmdText = @"insert into [Sys_Enum] (EnumCode,EnumName,EnumValue,ParentId,Sort,Remark) 
                             values (@EnumCode,@EnumName,@EnumValue,@ParentId,@Sort,@Remark)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                       new SqlParameter("@EnumCode",SqlDbType.VarChar,50), 
                                       new SqlParameter("@EnumName",SqlDbType.NVarChar,256),
                                       new SqlParameter("@EnumValue",SqlDbType.NVarChar,256), 
                                       new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@Sort",SqlDbType.Int),
                                       new SqlParameter("@Remark",SqlDbType.NVarChar,256)
                                   };
            parms[0].Value = model.EnumCode;
            parms[1].Value = model.EnumName;
            parms[2].Value = model.EnumValue;
            parms[3].Value = Guid.Parse(model.ParentId.ToString());
            parms[4].Value = model.Sort;
            parms[5].Value = model.Remark;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysEnumInfo model)
        {
            if (model == null) return -1;

            if (IsExist(model.EnumName,model.ParentId, model.Id)) return 110;

            //定义查询命令
            string cmdText = @"update [Sys_Enum] set EnumCode = @EnumCode,EnumName = @EnumName,EnumValue = @EnumValue,ParentId = @ParentId,Sort = @Sort,Remark = @Remark 
                             where Id = @Id";

            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@EnumCode",SqlDbType.VarChar,50), 
                                     new SqlParameter("@EnumName",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@EnumValue",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@Sort",SqlDbType.Int),
                                     new SqlParameter("@Remark",SqlDbType.NVarChar,256)
                                   };
            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = model.EnumCode;
            parms[2].Value = model.EnumName;
            parms[3].Value = model.EnumValue;
            parms[4].Value = Guid.Parse(model.ParentId.ToString());
            parms[5].Value = model.Sort;
            parms[6].Value = model.Remark;

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

            string cmdText = "delete from Sys_Enum where Id = @Id";
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
                sb.Append(@"delete from [Sys_Enum] where Id = @Id" + n + " ;");
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
        public SysEnumInfo GetModel(object Id)
        {
            SysEnumInfo model = null;

            string cmdText = @"select top 1 Id,EnumCode,EnumName,EnumValue,ParentId,Sort,Remark from [Sys_Enum] where Id = @Id ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new SysEnumInfo();
                        model.Id = reader["Id"];
                        model.EnumCode = reader.GetString(1);
                        model.EnumName = reader.GetString(2);
                        model.EnumValue = reader.GetString(3);
                        model.ParentId = reader[4];
                        model.Sort = reader.GetInt32(5);
                        model.Remark = reader.GetString(6).Trim();
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 是否存在对应数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool IsExist(string name, object parentId, object Id)
        {
            Guid gId = Guid.Empty;
            if (Id != null)
            {
                if (!Guid.TryParse(Id.ToString(), out gId))
                {
                    return false;
                }
            }

            ParamsHelper parms = new ParamsHelper();

            string cmdText = "select 1 from [Sys_Enum] where (lower(EnumName) = @EnumName or lower(EnumCode) = @EnumCode) and ParentId = @ParentId";
            if (!gId.Equals(Guid.Empty))
            {
                cmdText = "select 1 from [Sys_Enum] where (lower(EnumName) = @EnumName or lower(EnumCode) = @EnumCode) and ParentId = @ParentId and Id <> @Id ";
                SqlParameter parm1 = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
                parm1.Value = gId;
                parms.Add(parm1);
            }
            SqlParameter parm = new SqlParameter("@EnumName", SqlDbType.NVarChar, 256);
            parm.Value = name;
            parms.Add(parm);
            parm = new SqlParameter("@EnumCode", SqlDbType.VarChar, 50);
            parm.Value = name;
            parms.Add(parm);

            parm = new SqlParameter("@ParentId", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(parentId.ToString());
            parms.Add(parm);

            object obj = SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms.ToArray());
            if (obj != null) return true;

            return false;
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
        public List<SysEnumInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Sys_Enum] t1 join dbo.Sys_Enum t2 on t1.ParentId = t2.Id ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.EnumCode,t1.EnumName,t1.EnumValue,t1.ParentId,t1.Sort,t1.Remark 
                      from [Sys_Enum] t1 join dbo.Sys_Enum t2 on t1.ParentId = t2.Id ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SysEnumInfo> list = new List<SysEnumInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SysEnumInfo model = new SysEnumInfo();
                        model.Id = reader["Id"];
                        model.EnumCode = reader.GetString(2);
                        model.EnumName = reader.GetString(3);
                        model.EnumValue = reader.GetString(4);
                        model.ParentId = reader[5];
                        model.Sort = reader.GetInt32(6);
                        model.Remark = reader.GetString(7).Trim();

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
        public List<SysEnumInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by t1.Sort) as RowNumber,t1.Id,t1.EnumCode,t1.EnumName,t1.EnumValue,t1.ParentId,t1.Sort,t1.Remark 
                             from [Sys_Enum] t1 join dbo.Sys_Enum t2 on t1.ParentId = t2.Id ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<SysEnumInfo> list = new List<SysEnumInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SysEnumInfo model = new SysEnumInfo();
                        model.Id = reader["Id"];
                        model.EnumCode = reader.GetString(2);
                        model.EnumName = reader.GetString(3);
                        model.EnumValue = reader.GetString(4);
                        model.ParentId = reader[5];
                        model.Sort = reader.GetInt32(6);
                        model.Remark = reader.GetString(7).Trim();

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<SysEnumInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            string cmdText = @"select t1.Id,t1.EnumCode,t1.EnumName,t1.EnumValue,t1.ParentId,t1.Sort,t1.Remark 
                             from [Sys_Enum] t1 join [Sys_Enum] t2 on t1.ParentId = t2.Id ";

            if (!string.IsNullOrEmpty(sqlWhere))
            {
                cmdText += " where 1=1 " + sqlWhere;
            }

            cmdText += " order by Sort ";

            List<SysEnumInfo> list = new List<SysEnumInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SysEnumInfo model = new SysEnumInfo();
                        model.Id = reader["Id"];
                        model.EnumCode = reader.GetString(1);
                        model.EnumName = reader.GetString(2);
                        model.EnumValue = reader.GetString(3);
                        model.ParentId = reader[4];
                        model.Sort = reader.GetInt32(5);
                        model.Remark = reader.GetString(6).Trim();

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
        public List<SysEnumInfo> GetList()
        {
            string cmdText = @"select Id,EnumCode,EnumName,EnumValue,ParentId,Sort,Remark 
                             from [Sys_Enum] order by Sort ";

            List<SysEnumInfo> list = new List<SysEnumInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SysEnumInfo model = new SysEnumInfo();
                        model.Id = reader["Id"];
                        model.EnumCode = reader.GetString(1);
                        model.EnumName = reader.GetString(2);
                        model.EnumValue = reader.GetString(3);
                        model.ParentId = reader[4];
                        model.Sort = reader.GetInt32(5);
                        model.Remark = reader.GetString(6).Trim();

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取树json格式字符串
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson()
        {
            StringBuilder jsonAppend = new StringBuilder();
            List<SysEnumInfo> list = GetList();
            if (list != null && list.Count > 0)
            {
                CreateTreeJson(list, Guid.Empty, ref jsonAppend);
            }
            else
            {
                jsonAppend.Append("[{\"id\":\"" + Guid.Empty + "\",\"text\":\"请选择\",\"state\":\"open\",\"attributes\":{\"parentId\":\"" + Guid.Empty + "\",\"parentName\":\"请选择\"}}]");
            }

            return jsonAppend.ToString();
        }

        /// <summary>
        /// 获取树json格式字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <param name="jsonAppend"></param>
        private void CreateTreeJson(List<SysEnumInfo> list, object parentId, ref StringBuilder jsonAppend)
        {
            jsonAppend.Append("[");
            var childList = list.FindAll(x => x.ParentId.Equals(parentId));
            if (childList.Count > 0)
            {
                int temp = 0;
                foreach (var model in childList)
                {
                    jsonAppend.Append("{\"id\":\"" + model.Id + "\",\"text\":\"" + model.EnumName + "\",\"state\":\"open\",\"attributes\":{\"parentId\":\"" + model.ParentId + "\",\"enumCode\":\"" + model.EnumCode + "\",\"enumValue\":\"" + model.EnumValue + "\",\"sort\":\"" + model.Sort + "\",\"remark\":\""+model.Remark+"\"}");
                    if (list.Any(r => r.ParentId.Equals(model.Id)))
                    {
                        jsonAppend.Append(",\"children\":");
                        CreateTreeJson(list, model.Id, ref jsonAppend);
                    }
                    jsonAppend.Append("}");
                    if (temp < childList.Count - 1) jsonAppend.Append(",");
                    temp++;
                }
            }
            jsonAppend.Append("]");
        }

        #endregion
    }
}
