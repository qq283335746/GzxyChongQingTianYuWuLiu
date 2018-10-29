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
    public class ContentType : IContentType
    {
        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(ContentTypeInfo model)
        {
            if (model == null) return -1;

            //判断当前记录是否存在，如果存在则返回;
            if (IsExist(model.TypeName, null)) return 110;

            string cmdText = "insert into [Sys_ContentType] (TypeName,ParentId,Sort,TypeCode,LastUpdatedDate) values (@TypeName,@ParentId,@Sort,@TypeCode,@LastUpdatedDate)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@TypeName",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@Sort",SqlDbType.Int), 
                                     new SqlParameter("@TypeCode",SqlDbType.NVarChar,50), 
                                     new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)  
                                   };
            parms[0].Value = model.TypeName;
            parms[1].Value = Guid.Parse(model.ParentId.ToString());
            parms[2].Value = model.Sort;
            parms[3].Value = model.TypeCode;
            parms[4].Value = model.LastUpdatedDate;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ContentTypeInfo model)
        {
            if (model == null) return -1;

            if (IsExist(model.TypeName, model.Id)) return 110;

            //定义查询命令
            string cmdText = @"update [Sys_ContentType] set TypeName = @TypeName,ParentID = @ParentID,Sort = @Sort,TypeCode = @TypeCode,LastUpdatedDate = @LastUpdatedDate where Id = @Id ";

            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@TypeName",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@Sort",SqlDbType.Int), 
                                     new SqlParameter("@TypeCode",SqlDbType.NVarChar,50), 
                                     new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = model.TypeName;
            parms[2].Value = Guid.Parse(model.ParentId.ToString());
            parms[3].Value = model.Sort;
            parms[4].Value = model.TypeCode;
            parms[5].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="numberId"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            if (Id == null) return -1;
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return -1;

            string cmdText = "delete from Sys_ContentType where Id = @Id ";
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
                sb.Append(@"delete from [Sys_ContentType] where Id = @Id" + n + " ;");
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
        /// <param name="numberId"></param>
        /// <returns></returns>
        public ContentTypeInfo GetModel(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return null;

            ContentTypeInfo model = new ContentTypeInfo();

            string cmdText = @"select top 1 Id,TypeName,ParentId,Sort,TypeCode,LastUpdatedDate from [Sys_ContentType] where Id = @Id order by LastUpdatedDate desc ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model.Id = reader.GetGuid(0);
                        model.TypeName = reader.GetString(1);
                        model.ParentId = reader.GetGuid(2);
                        model.Sort = reader.GetInt32(3);
                        model.TypeCode = reader.GetString(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);
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
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public List<ContentTypeInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Sys_ContentType] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,Id,TypeName,ParentID,Sort,TypeCode,LastUpdatedDate from [Sys_ContentType] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ContentTypeInfo> list = new List<ContentTypeInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ContentTypeInfo model = new ContentTypeInfo();
                        model.Id = reader.GetGuid(1);
                        model.TypeName = reader.GetString(2);
                        model.ParentId = reader.GetGuid(3);
                        model.Sort = reader.GetInt32(4);
                        model.TypeCode = reader.GetString(5);
                        model.LastUpdatedDate = reader.GetDateTime(6);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<ContentTypeInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by Sort) as RowNumber,Id,TypeName,ParentId,Sort,TypeCode,LastUpdatedDate from [Sys_ContentType] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ContentTypeInfo> list = new List<ContentTypeInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ContentTypeInfo model = new ContentTypeInfo();
                        model.Id = reader.GetGuid(0);
                        model.TypeName = reader.GetString(1);
                        model.ParentId = reader.GetGuid(2);
                        model.Sort = reader.GetInt32(3);
                        model.TypeCode = reader.GetString(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<ContentTypeInfo> GetList()
        {
            List<ContentTypeInfo> list = new List<ContentTypeInfo>();

            string cmdText = "select Id,TypeName,ParentId,Sort,TypeCode,LastUpdatedDate from [Sys_ContentType] ";

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ContentTypeInfo model = new ContentTypeInfo();
                        model.Id = reader.GetGuid(0);
                        model.TypeName = reader.GetString(1);
                        model.ParentId = reader.GetGuid(2);
                        model.Sort = reader.GetInt32(3);
                        model.TypeCode = reader.GetString(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 是否存在对应数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numberId"></param>
        /// <returns></returns>
        public bool IsExist(string name, object numberId)
        {
            Guid gId = Guid.Empty;
            if (numberId != null)
            {
                Guid.TryParse(numberId.ToString(), out gId);
            }

            ParamsHelper parms = new ParamsHelper();

            string cmdText = "select 1 from [Sys_ContentType] where lower(TypeName) = @TypeName";
            if (gId != Guid.Empty)
            {
                cmdText = "select 1 from [Sys_ContentType] where lower(TypeName) = @TypeName and Id <> @Id ";
                SqlParameter parm1 = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
                parm1.Value = Guid.Parse(numberId.ToString());
                parms.Add(parm1);
            }
            SqlParameter parm = new SqlParameter("@TypeName", SqlDbType.NVarChar, 256);
            parm.Value = name.ToLower();
            parms.Add(parm);

            object obj = SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms.ToArray());
            if (obj != null) return true;

            return false;
        }

        /// <summary>
        /// 获取树json数据
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson()
        {
            StringBuilder jsonAppend = new StringBuilder();
            List<ContentTypeInfo> list = GetList();
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
        private void CreateTreeJson(List<ContentTypeInfo> list, object parentId, ref StringBuilder jsonAppend)
        {
            jsonAppend.Append("[");
            var childList = list.FindAll(x => x.ParentId.Equals(parentId));
            if (childList.Count > 0)
            {
                int temp = 0;
                foreach (var model in childList)
                {
                    jsonAppend.Append("{\"id\":\"" + model.Id + "\",\"text\":\"" + model.TypeName + "\",\"state\":\"open\",\"attributes\":{\"parentId\":\"" + model.ParentId + "\",\"typeCode\":\"" + model.TypeCode + "\",\"sort\":\"" + model.Sort + "\"}");
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
    }
}
